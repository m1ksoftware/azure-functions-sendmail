namespace AzureSendMail
{
    using FluentValidation;

    /// <summary>
    /// Custum validator for email data
    /// </summary>
    public class MailDataValidator : AbstractValidator<MailData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailDataValidator"/>
        /// class.
        /// </summary>
        public MailDataValidator()
        {
            RuleFor(x => x.FromMail)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("FromMail invalid");

            RuleFor(x => x.FromName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2)
                .WithMessage("FromName too short");

            RuleFor(x => x.Subject)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("Subject invalid or too short");

            RuleFor(x => x.Message)
                .NotNull()
                .NotEmpty()
                .MinimumLength(10)
                .WithMessage("Message invalud or too short");

            RuleFor(x => x.PrivacyPolicyAccepted)
                .Must(x => x)
                .WithMessage("Privacy Policy not accepted");
        }
    }
}

