using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Exception;

public class InvalidWishlistException(Wishlist wishlist, int id)
    : System.Exception($"There is no ID {id} in the wishlist wish owner ID {wishlist.EmployeeId}");
