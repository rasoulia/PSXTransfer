using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSXTransfer.WPF.MVVM.Models
{
    public class Game
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string? Title { get; set; }

        public string? TitleID { get; set; }

        public string? LocalPath { get; set; }

        public int Region { get; set; }

        public string? Version { get; set; }

        public string? Console { get; set; }

        public string? Link { get; set; }
    }
}
