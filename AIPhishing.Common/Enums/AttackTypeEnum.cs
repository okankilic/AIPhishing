using System.ComponentModel;

namespace AIPhishing.Common.Enums;

public enum AttackTypeEnum
{
    [Description("Urgent Account Required")]
    UrgentAccountRequired = 1,
    [Description("Security Alert")]
    SecurityAlert,
    [Description("Payment Confirmation")]
    PaymentConfirmation,
    [Description("Unusual Login Attempt")]
    UnusualLoginAttempt,
    [Description("Invoice Attached")]
    InvoiceAttached,
    [Description("Password Reset Request")]
    PasswordResetRequest,
    [Description("Package Delivery Notification")]
    PackageDeliveryNotification,
    [Description("Important Message From")]
    ImportantMessageFrom,
    [Description("Account Suspension Warning")]
    AccountSuspensionWarning,
    [Description("Message From IT Support")]
    MessageFromITSupport,
    [Description("Subscription Renewal Notice")]
    SubscriptionRenewalNotice
}