namespace MojePszczoly.Infrastructure.Entities
{
    public class Bread
    {
        public int BreadId { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public int SortOrder { get; set; }
    }
}
