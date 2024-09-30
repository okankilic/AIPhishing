using System.Globalization;
using AIPhishing.Business.Clients.Models;
using AIPhishing.Business.Contexts;
using AIPhishing.Common.Exceptions;
using AIPhishing.Common.Helpers;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AIPhishing.Business.Clients;

public class ClientBusiness : IClientBusiness
{
    private readonly PhishingDbContext _dbContext;

    public ClientBusiness(PhishingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task CreateAsync(ClientCreateRequest request, UserContext currentUser)
    {
        if (currentUser is not { IsGodUser: true })
            throw new BusinessException($"You cannot take this action.");
        
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (string.IsNullOrEmpty(request.ClientName))
            throw BusinessException.Required(nameof(request.ClientName));

        if (await _dbContext.Clients.AnyAsync(q => q.ClientName.ToLower() == request.ClientName.ToLowerInvariant()))
            throw BusinessException.InUse(nameof(Client.ClientName), request.ClientName);
        
        if (request.User == null)
            throw BusinessException.Required(nameof(request.User));
        
        if (string.IsNullOrEmpty(request.User.Email))
            throw BusinessException.Required(nameof(request.User.Email));
        
        if (string.IsNullOrEmpty(request.User.Password))
            throw BusinessException.Required(nameof(request.User.Password));

        if (await _dbContext.Users.AnyAsync(q => q.Email == request.User.Email))
            throw new BusinessException($"User {request.User.Email} is already defined.");
        
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
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        var count = await _dbContext.Clients
            .Where(q => currentUser.IsGodUser || q.Id == currentUser.ClientId)
            .CountAsync();

        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;
        
        var clients = await _dbContext.Clients
            .Where(q => currentUser.IsGodUser || q.Id == currentUser.ClientId)
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

    public async Task<ClientTargetListResponse> ListTargetsAsync(Guid clientId, ClientTargetListRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        var count = await _dbContext.ClientTargets.CountAsync(q => q.ClientId == clientId);

        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;
        
        var targets = await _dbContext.ClientTargets
            .Where(q => q.ClientId == clientId)
            .Select(q => new
            {
                q.Id,
                q.Email,
                q.FullName,
                q.CreatedAt
            })
            .OrderByDescending(q => q.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToArrayAsync();

        var response = (from target in targets
                select new ClientTargetListViewModel(target.Id, target.Email, target.FullName))
            .ToArray();

        return new ClientTargetListResponse(response, count);
    }

    public async Task<ClientViewModel> GetAsync(Guid clientId, UserContext currentUser)
    {
        var client = await _dbContext.Clients
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == clientId)
                     ?? throw BusinessException.NotFound(nameof(Client), clientId);

        var clientUser = await _dbContext.Users
                             .AsNoTracking()
                             .SingleOrDefaultAsync(q => q.ClientId == clientId)
                         ?? throw new BusinessException($"User not found for Client: {client.ClientName}");

        return new ClientViewModel(
            client.Id,
            client.ClientName,
            new ClientUserViewModel(clientUser.Id, clientUser.Email));
    }

    public async Task UpdateAsync(Guid clientId, ClientUpdateRequest request, UserContext currentUser)
    {
        if (currentUser is not { IsGodUser: true })
            throw new BusinessException($"You cannot take this action.");
        
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (string.IsNullOrEmpty(request.ClientName))
            throw BusinessException.Required(nameof(request.ClientName));

        if (_dbContext.Clients.Any(q => q.Id != clientId && q.ClientName.ToLower() == request.ClientName.ToLowerInvariant()))
            throw BusinessException.InUse(nameof(Client.ClientName), request.ClientName);

        var client = await _dbContext.Clients
                         .SingleOrDefaultAsync(q => q.Id == clientId)
                     ?? throw BusinessException.NotFound(nameof(Client), clientId);

        client.ClientName = request.ClientName;

        _dbContext.Clients.Update(client);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(Guid clientId, ClientUserEditModel request, UserContext currentUser)
    {
        if (currentUser is not { IsGodUser: true })
            throw new BusinessException($"You cannot take this action.");
        
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        if (string.IsNullOrEmpty(request.Email))
            throw BusinessException.Required(nameof(request.Email));
        
        if (string.IsNullOrEmpty(request.Password))
            throw BusinessException.Required(nameof(request.Password));

        if (await _dbContext.Users.AnyAsync(q => q.ClientId != clientId & q.Email == request.Email))
            throw new BusinessException($"User {request.Email} is already defined.");
        
        var client = await _dbContext.Clients
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == clientId)
                     ?? throw BusinessException.NotFound(nameof(Client), clientId);
        
        var clientUser = await _dbContext.Users
                             .SingleOrDefaultAsync(q => q.ClientId == clientId)
                         ?? throw new BusinessException($"User not found for Client: {client.ClientName}");

        clientUser.Email = request.Email;
        clientUser.Password = PasswordHelper.Hash(request.Password);

        _dbContext.Users.Update(clientUser);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ImportTargetsAsync(Guid clientId, IFormFile file, UserContext currentUser)
    {
        if (!currentUser.IsGodUser && clientId != currentUser.ClientId)
            throw new BusinessException($"You cannot take this action.");
        
        if (file == null || Path.GetExtension(file.FileName) != ".csv")
            throw BusinessException.Required(nameof(file));
        
        var client = await _dbContext.Clients
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == clientId)
                     ?? throw BusinessException.NotFound(nameof(Client), clientId);

        ClientTargetCsvModel[] targets = [];
        
        try
        {
            await using var stream = file.OpenReadStream();
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
            throw BusinessException.Invalid(nameof(file));
        }
        
        if (targets.Length == 0)
            throw BusinessException.Invalid(nameof(file));

        using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingTargets = await _dbContext.ClientTargets
                .Where(q => q.ClientId == clientId)
                .ToArrayAsync();

            foreach (var target in targets)
            {
                var existingTarget = existingTargets.SingleOrDefault(q => q.Email == target.Email);

                if (existingTarget != null)
                {
                    existingTarget.FullName = target.FullName;

                    _dbContext.ClientTargets.Update(existingTarget);
                }
                else
                {
                    var clientTarget = new ClientTarget
                    {
                        Id = Guid.NewGuid(),
                        ClientId = client.Id,
                        Email = target.Email,
                        FullName = target.FullName,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dbContext.ClientTargets.AddAsync(clientTarget);
                }
            }

            await _dbContext.SaveChangesAsync();

            await ts.CommitAsync();
        }
        catch (Exception)
        {
            await ts.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteTargetAsync(Guid clientId, Guid targetId, UserContext currentUser)
    {
        if (!currentUser.IsGodUser && clientId != currentUser.ClientId)
            throw new BusinessException($"You cannot take this action.");

        var target = await _dbContext.ClientTargets
                         .SingleOrDefaultAsync(q => q.ClientId == clientId && q.Id == targetId)
                     ?? throw BusinessException.NotFound(nameof(ClientTarget), targetId);

        _dbContext.ClientTargets.Remove(target);

        await _dbContext.SaveChangesAsync();
    }
}