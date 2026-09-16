using System;

namespace Tweaker.Models
{
    public class PersistenceItem
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsSignedByMicrosoft { get; set; }
        public string StatusBadge => IsSignedByMicrosoft ? "Microsoft / Seguro" : "Tercero / No Verificado";
        public string BadgeColor => IsSignedByMicrosoft ? "#107C10" : "#D83B01";
    }

    public class HostsEntry
    {
        public int LineNumber { get; set; }
        public string IP { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool IsSuspicious { get; set; }
        public string RawLine { get; set; } = string.Empty;
    }

    public class NetworkConnectionItem
    {
        public string ProcessName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public string Protocol { get; set; } = "TCP";
        public string LocalAddress { get; set; } = string.Empty;
        public string RemoteAddress { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool IsSuspicious { get; set; }
    }
}

