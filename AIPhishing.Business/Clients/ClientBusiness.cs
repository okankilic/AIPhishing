using System.Globalization;
using AIPhishing.Business.Clients.Models;
using AIPhishing.Business.Contexts;
using AIPhishing.Common.Exceptions;
using AIPhishing.Common.Helpers;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AIPhishing.Business.Clients;

public class ClientBusiness : IClientBusiness
{
    private readonly PhishingDbContext _dbContext;

    public ClientBusiness(PhishingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task CreateAsync(ClientEditRequest request, UserContext currentUser)
    {
        if (currentUser is not { IsGodUser: true })
            throw new UnauthorizedAccessException($"You cannot take this action.");
        
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (string.IsNullOrEmpty(request.ClientName))
            throw BusinessException.Required(nameof(request.ClientName));

        if (_dbContext.Clients.Any(q => q.ClientName.ToLower() == request.ClientName.ToLowerInvariant()))
            throw BusinessException.InUse(nameof(Client.ClientName), request.ClientName);
        
        if (request.User == null)
            throw BusinessException.Required(nameof(request.User));
        
        if (string.IsNullOrEmpty(request.User.Email))
            throw BusinessException.Required(nameof(request.User.Email));
        
        if (string.IsNullOrEmpty(request.User.Password))
            throw BusinessException.Required(nameof(request.User.Password));
        
        // TODO: Validate password agains length, special chars, etc.
        if (request.CsvFile == null || Path.GetExtension(request.CsvFile.FileName) != ".csv")
            throw BusinessException.Required(nameof(request.CsvFile));

        ClientTargetCsvModel[] targets = [];
        
        try
        {
            await using var stream = request.CsvFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            };
            using var csv = new CsvReader(reader, csvConfiguration);
            targets = csv.GetRecords<ClientTargetCsvModel>().ToArray();
        }
        catch (Exception)
        {
            throw BusinessException.Invalid(nameof(request.CsvFile));
        }
        
        if (targets.Length == 0)
            throw BusinessException.Invalid(nameof(request.CsvFile));
        
        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                ClientName = request.ClientName,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Clients.AddAsync(client);

            await _dbContext.SaveChangesAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                ClientId = client.Id,
                Email = request.User.Email,
                Password = PasswordHelper.Hash(request.User.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Users.AddAsync(user);
            
            await _dbContext.SaveChangesAsync();

            var clientTargets = targets
                .Select(t => new ClientTarget
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    Email = t.Email,
                    FullName = t.FullName,
                    CreatedAt = DateTime.UtcNow
                });

            await _dbContext.ClientTargets.AddRangeAsync(clientTargets);

            await _dbContext.SaveChangesAsync();
            
            await ts.CommitAsync();
        }
        catch (Exception)
        {
            await ts.RollbackAsync();
            throw;
        }
    }

    public async Task<ClientListResponse> ListAsync(ClientListRequest request, UserContext currentUser)
    {
        if (currentUser is not { IsGodUser: true })
            throw new UnauthorizedAccessException($"You cannot take this action.");
        
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        var count = await _dbContext.Clients.CountAsync();

        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;
        
        var clients = await _dbContext.Clients
            .Select(q => new
            {
                q.Id,
                q.ClientName,
                q.CreatedAt
            })
            .OrderByDescending(q => q.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToArrayAsync();

        var response = (from client in clients
                select new ClientListViewModel(client.Id, client.ClientName, client.CreatedAt))
            .ToArray();

        return new ClientListResponse(response, count);
    }
}