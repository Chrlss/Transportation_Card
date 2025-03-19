using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Transportation_Card.Models;
using Transportation_Card.Data;
using Org.BouncyCastle.Utilities.Net;
using Microsoft.EntityFrameworkCore;
using Transportation_Card.Migrations;

namespace Transportation_Card.Services
{
    public class AuthService
    {
        private readonly UsersDbContext _context;

        public AuthService()
        {
            _context = new UsersDbContext();
        }

        public string? SeniorCitizenCard { get; set; }
        public string? PwdId { get; set; }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<User> AuthenticateAsync(string username, string email, string password)
        {
            var user = await _context.Users
                .Where(u => (u.Username == username || u.Email == email) && u.PasswordHash == password)
                .FirstOrDefaultAsync();

            return user;
        }

        public void Register(string fullName, string username, string email, string password, string address, DateTime dateOfBirth, string mobileNumber)
        {
            using (var context = new UsersDbContext())
            {
                var cardNumber = GenerateCardNumber();
                while (_context.Users.Any(u => u.CardNumber == cardNumber))
                {
                    cardNumber = GenerateCardNumber();
                }

                var newUser = new User
                {
                    FullName = fullName,
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    Address = address,
                    DateOfBirth = dateOfBirth,
                    MobileNumber = mobileNumber,
                    CardNumber = cardNumber
                };

                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        public void RegisterDiscountedCard(string fullName, string username, string email, string password, string address, DateTime dateOfBirth, string mobileNumber, string cardType, string cardNumber, string SeniorCitizenCard, string PwdId)
        {
            using (var context = new UsersDbContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    throw new ArgumentException("Username already exists.");
                }

                if (cardType == "SeniorCitizen" && !ValidateSeniorCitizenCard(cardNumber))
                {
                    throw new ArgumentException("Invalid Senior Citizen Card format.");
                }

                if (cardType == "PWD" && !ValidatePwdId(cardNumber))
                {
                    throw new ArgumentException("Invalid PWD ID format.");
                }

                var cardNumberGenerated = cardNumber;  // Use the provided card number instead of generating a new one

                var newUser = new User
                {
                    FullName = fullName,
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    Address = address,
                    DateOfBirth = dateOfBirth,
                    MobileNumber = mobileNumber,
                    CardNumber = cardNumberGenerated,
                    InitialLoad = 500m,
                    LastUsedDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddYears(3),
                    PwdId = cardType == "SeniorCitizen" ? cardNumber : string.Empty,
                    SeniorCitizenCard = cardType == "PWD" ? cardNumber : string.Empty
                };

                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        public string GenerateCardNumber()
        {
            string prefix = "C-";
            var lastUser = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.CardNumber))
                .OrderByDescending(u => u.ID)
                .FirstOrDefault();

            int nextNumber = 1;  // Default to 1 if no user or card is found

            if (lastUser != null && !string.IsNullOrEmpty(lastUser.CardNumber))
            {
                var lastCardNumber = lastUser.CardNumber.Substring(2);  // Get the part after "C-"

                if (lastCardNumber.Length > 0 && int.TryParse(lastCardNumber, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;  // Increment by 1 if valid number is found
                }
                else
                {
                    // Handle case where last card number part isn't a valid number
                    nextNumber = 1; // Reset to 1 if there's a problem with the previous card number
                }
            }

            // Return the generated card number formatted to 12 digits
            return "C-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }

        public bool ReloadCard(int userId, decimal amount)
        {
            if (amount < 100 || amount > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Reload amount must be between P100 and P5000.");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.InitialLoad += amount;
            _context.SaveChanges();

            return true;
        }

        public User? Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            var user = _context.Users
                .Where(u => u.Username == username && u.PasswordHash == hashedPassword)
                .Select(u => new User
                {
                    ID = u.ID,
                    CardNumber = u.CardNumber,
                    FullName = u.FullName,
                    Username = u.Username,
                    DateOfBirth = u.DateOfBirth,
                    Address = u.Address,
                    MobileNumber = u.MobileNumber,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    InitialLoad = u.InitialLoad,
                    LastUsedDate = u.LastUsedDate,
                    ExpirationDate = u.ExpirationDate,
                    CreatedAt = u.CreatedAt,
                    SeniorCitizenCard = u.SeniorCitizenCard ?? string.Empty,  // Handle NULL
                    PwdId = u.PwdId ?? string.Empty
                })
                .FirstOrDefault();

            if (user != null)
            {
                // Use a local variable to store the formatted date
                var createdAtFormatted = user.CreatedAt.ToString("MM/dd/yy");
                var expirationDateFormatted = user.ExpirationDate?.ToString("MM/dd/yy");
            }

            return user;
        }

        public User GetUserFromDatabase(int userId)
        {
            using (_context)
            {
                return _context.Users.FirstOrDefault(u => u.ID == userId); // Assuming userId is primary key
            }
        }

        public void UseCard(int userId, decimal fare, decimal exitFare)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Check if the user has enough initial load
            if (user.InitialLoad < fare)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            // Deduct the fare from the initial load
            user.InitialLoad -= fare;

            // Set the LastUsedDate if it hasn't been set yet (i.e., this is the first time the card is used)
            if (!user.LastUsedDate.HasValue)
            {
                user.LastUsedDate = DateTime.Now; // Set to the current date and time
            }

            // Set the ExpirationDate based on LastUsedDate. If it was set already, update it.
            if (user.LastUsedDate.HasValue)
            {
                user.ExpirationDate = user.LastUsedDate.Value.AddYears(5); // Set expiration 5 years after LastUsedDate
            }

            // Save changes to the database
            _context.SaveChanges();
        }

        public void ExitCard(int userId, decimal exitFare)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (user.InitialLoad < exitFare)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.InitialLoad -= exitFare;
            user.LastUsedDate = DateTime.Now;
            user.ExpirationDate = user.LastUsedDate?.AddYears(5);

            _context.SaveChanges();
        }

        private bool ValidateSeniorCitizenCard(string cardNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^\d{2}-\d{3}-\d{4}$");
        }

        private bool ValidatePwdId(string pwdId)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(pwdId, @"^\d{4}-\d{4}-\d{4}$");
        }
    }

}
