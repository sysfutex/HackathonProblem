using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Exceptions;

public class InvalidWishlistException(Wishlist wishlist, int id)
    : Exception($"There is no ID {id} in the wishlist with owner ID {wishlist.EmployeeId}");
