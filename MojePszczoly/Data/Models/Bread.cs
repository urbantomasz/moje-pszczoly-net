using System.ComponentModel.DataAnnotations;

namespace MojePszczoly.Data.Models
{
    public class Bread
    {
        public int BreadId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int SortOrder { get; set; }
    }
}
