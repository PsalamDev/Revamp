namespace HRShared.Common
{
    public class ClientMailRequest
    {
        public ClientMailRequest(string clientDisplayName, string clientFrom, string clientHost, int clientPort, string clientUsername,
            string clientPassword, List<string> to, string subject, string? body = null, string? from = null, string? displayName = null,
            string? replyTo = null, string? replyToName = null, List<string>? bcc = null, List<string>? cc = null,
            IDictionary<string, byte[]>? attachmentData = null, IDictionary<string, string>? headers = null)
        {
            ClientDisplayName = clientDisplayName;
            ClientFrom = clientFrom;
            ClientHost = clientHost;
            ClientPort = clientPort;
            ClientUsername = clientUsername;
            ClientPassword = clientPassword;
            To = to;
            Subject = subject;
            Body = body;
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();
            AttachmentData = attachmentData ?? new Dictionary<string, byte[]>();
            Headers = headers ?? new Dictionary<string, string>();
        }

        public string ClientDisplayName { get; }
        public string ClientFrom { get; }
        public string ClientHost { get; }
        public int ClientPort { get; }
        public string ClientPassword { get; }
        public string ClientUsername { get; }
        public List<string> To { get; }

        public string Subject { get; }

        public string? Body { get; }

        public string? From { get; }

        public string? DisplayName { get; }

        public string? ReplyTo { get; }

        public string? ReplyToName { get; }

        public List<string> Bcc { get; }

        public List<string> Cc { get; }

        public IDictionary<string, byte[]> AttachmentData { get; }

        public IDictionary<string, string> Headers { get; }
    }
}