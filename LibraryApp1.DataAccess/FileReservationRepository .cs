using System.Text.Json;

namespace LibraryApp1.DataAccess
{
    public class FileReservationRepository : IReservationRepository
    {
        private readonly string filePath = "reservations.json";

        private List<Reservation> ReadFile()
        {
            if (!File.Exists(filePath))
                return new List<Reservation>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
        }

        private void WriteFile(List<Reservation> reservations)
        {
            string json = JsonSerializer.Serialize(reservations,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public Reservation? GetById(int id)
        {
            return ReadFile().Find(r => r.Id == id);
        }

        public Reservation? GetActiveByBookId(int bookId)
        {
            return ReadFile().Find(r => r.BookId == bookId && r.ReturnedAt == null);
        }

        public List<Reservation> GetAll(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null)
        {
            IEnumerable<Reservation> query = ReadFile();

            if (isActive.HasValue)
                query = query.Where(r => r.IsActive == isActive.Value);

            if (hasFine.HasValue)
                query = query.Where(r => hasFine.Value ? r.Fine > 0 : r.Fine == 0);

            if (!string.IsNullOrWhiteSpace(borrowerName))
                query = query.Where(r =>
                    r.BorrowerName.Contains(borrowerName, StringComparison.OrdinalIgnoreCase));

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void Add(Reservation reservation)
        {
            var reservations = ReadFile();
            reservation.Id = reservations.Count == 0 ? 1 : reservations.Max(r => r.Id) + 1;
            reservations.Add(reservation);
            WriteFile(reservations);
        }

        public void Update(Reservation reservation)
        {
            var reservations = ReadFile();
            int index = reservations.FindIndex(r => r.Id == reservation.Id);
            if (index == -1) return;

            reservations[index] = reservation;
            WriteFile(reservations);
        }

        public void Delete(int id)
        {
            var reservations = ReadFile();
            reservations.RemoveAll(r => r.Id == id);
            WriteFile(reservations);
        }
    }
}