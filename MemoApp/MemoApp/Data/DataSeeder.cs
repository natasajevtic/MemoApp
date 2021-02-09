using System.Linq;

namespace MemoApp.Data
{
    public class DataSeeder
    {
        private readonly MemoEntities _entities;

        public DataSeeder(MemoEntities entities)
        {
            _entities = entities;
        }

        public void SeedStatusData()
        {
            if (!_entities.Statuses.Any())
            {
                _entities.Statuses.AddRange(
                new Status
                {
                    Id = 1,
                    Name = "Active",
                    Description = "The memo is active."
                },
                new Status
                {
                    Id = 2,
                    Name = "Deleted",
                    Description = "The memo is deleted."
                }
            );
                _entities.SaveChanges();
            }
        }
    }
}
