using AIPhishing.Business.Integrations;
using AIPhishing.Business.Integrations.Models;
using AIPhishing.Common.Enums;
using Microsoft.Extensions.Configuration;

namespace AIPhishing.Business.Mocks;

public class MockPhishingAiApiClient : IPhishingAiApiClient
{
    private readonly IConfiguration _configuration;

    public MockPhishingAiApiClient(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<PhishingAiGetEmailContentResponse> CreateEmailContentAsync(string language, PhishingAiGetEmailContentRequest request)
    {
        await Task.Delay(3000);

        // throw new Exception($"Something happened");

        var appUrl = _configuration.GetValue<string>("ApiBaseUrl")!;
        var linkUrl = $"{appUrl}/api/webhooks/clicked/{request.EmailId}";

        var subject = "You gained a gift";
        var body = string.Empty;

        switch (Enum.Parse<AttackTypeEnum>(request.AttackType))
        {
            case AttackTypeEnum.UrgentAccountRequired:
                body = $"""
                        Dear {request.TargetFullName},
                        <br>
                        <br>
                        We have detected unusual activity in your account and require immediate verification to ensure your account's security.
                        <br>
                        <br>
                        Please update your account information by clicking the link below:
                        <br>
                        <br>
                        <a href="{linkUrl}">[Update Account Information]</a>
                        <br>
                        <br>
                        Failure to update your information within 24 hours may result in temporary suspension of your account.
                        <br>
                        <br>
                        Thank you for your prompt attention to this matter.
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Security Team
                        """;
                break;
            
            case AttackTypeEnum.SecurityAlert:
                body = $"""
                        Dear {request.TargetFullName},
                        <br>
                        <br>
                        We have detected suspicious activity on your account. To ensure your security, please verify your account immediately.
                        <br>
                        <br>
                        Click here to review your account activity: <a href="{linkUrl}">[Review Account Activity]</a>
                        <br>
                        <br>
                        If this activity was not authorized by you, please contact our support team immediately.
                        <br>
                        <br>
                        Thank you for your attention.
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Security Team
                        """;
                break;
            
            case AttackTypeEnum.PaymentConfirmation:
                body = $"""
                        Dear {request.TargetFullName}, 
                        <br>
                        <br>
                        We are pleased to inform you that your payment of [Amount] has been successfully processed. 
                        <br>
                        <br>
                        You can view your transaction details and receipt here: <a href="{linkUrl}">[View Transaction]</a> 
                        <br>
                        <br>
                        Thank you for your purchase! 
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Billing Team 
                        """;
                break;
            
            case AttackTypeEnum.UnusualLoginAttempt:
                body = $"""
                        Dear {request.TargetFullName}, 
                        <br>
                        <br>
                        We have detected an unusual login attempt on your account. If this was not you, please secure your account immediately. 
                        <br>
                        <br>
                        Review your login activity and secure your account: <a href="{linkUrl}">[Review Login Activity]</a>
                        <br>
                        <br>
                        If you need assistance, contact our support team. 
                        <br>
                        <br>
                        Thank you for your prompt attention. 
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Security Team 
                        """;
                break;
            
            case AttackTypeEnum.InvoiceAttached:
                body = $"""
                        Dear {request.TargetFullName}, 
                        <br>
                        <br>
                        Please find attached your invoice <a href="{linkUrl}">[Number]</a> for the recent transaction. Kindly review the details at your earliest convenience. 
                        <br>
                        <br>
                        If you have any questions or concerns, do not hesitate to contact us. 
                        <br>
                        <br>
                        Thank you for your prompt attention. 
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Billing Department 
                        """;
                break;
            
            case AttackTypeEnum.PasswordResetRequest:
                body = $"""
                        Dear {request.TargetFullName}, 
                        <br>
                        <br>
                        We have received a request to reset your password. If you made this request, please click the link below to reset your password. 
                        <br>
                        <br>
                        Reset your password here: <a href="{linkUrl}">[Reset Link]<a/> 
                        <br>
                        <br>
                        If you did not request a password reset, please ignore this email or contact our support team immediately. 
                        <br>
                        <br>
                        Thank you for your attention. 
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Support Team 
                        """;
                break;
            
            case AttackTypeEnum.PackageDeliveryNotification:
                body = $"""
                        Dear {request.TargetFullName}, 
                        <br>
                        <br>
                        We are excited to inform you that your package is on its way! You can track your package using the link below: 
                        <br>
                        <br>
                        Track Your Package: <a href="{linkUrl}">[Tracking Link]</a> 
                        <br>
                        <br>
                        If you have any questions, please feel free to contact our customer service team. 
                        <br>
                        <br>
                        Thank you for shopping with us! 
                        <br>
                        <br>
                        Best regards,
                        <br>
                        [Your Company's Name] Shipping Team 
                        """;
                break;
            
//             case AttackTypeEnum.JobOffer:
//                 body = $"""
//                         Dear {request.TargetFullName}, 
//                         <br>
//                         <br>
//                         We are thrilled to offer you an exciting job opportunity at [Company Name]. We believe your skills and experience would be a valuable addition to our team. 
//                         <br>
//                         <br>
//                         Please review the attached job offer letter for more details about the position and benefits. We look forward to hearing from you soon. 
//                         <br>
//                         <a href="{linkUrl}">Link</a>
//                         <br>
//                         <br>
//                         Congratulations, and we hope to welcome you on board! 
//                         <br>
//                         <br>
//                         Best regards,
//                         <br>
//                         [Your Name]
//                         <br>
//                         [Your Position]
//                         <br> 
//                         [Company Name] 
//                         """;
//                 break;
            
            case AttackTypeEnum.ImportantMessageFrom:
                body = "";
                break;
            
            case AttackTypeEnum.AccountSuspensionWarning:
                body = "";
                break;
            
            case AttackTypeEnum.MessageFromITSupport:
                body = "";
                break;
            
            case AttackTypeEnum.SubscriptionRenewalNotice:
                body = "";
                break;
        }

        var html = $"<html><body>{body}</body></html>";

        return new PhishingAiGetEmailContentResponse("tester@insangercektenhayretediyor.com",subject, html);
    }

    public Task<string> GetReplyEmailContentAsync(string language, PhishingAiGetReplyEmailContentRequest request)
    {
        return Task.FromResult<string>(string.Empty);
    }
}