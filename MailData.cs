namespace AzureSendMail
{
    /// <summary>
    /// Model for email data
    /// </summary>
    public class MailData
    {
        /// <summary>
        /// Gets or sets the sender email address 
        /// </summary>
        public string FromMail { get; set; }

        /// <summary>
        /// Gets or set the sender name
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or set the email address
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or set the email message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has accepted the
        /// website privacy policy and related stuff
        /// </summary>
        public bool PrivacyPolicyAccepted { get; set; }
    }
}

