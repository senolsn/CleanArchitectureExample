using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Insert(User user); //Insert isleminde bir db islemi olmadigi icin bunu Task ("Asenkron") olarak tan�mlamak gereksizdir. Repository Pattern'de genellikle;
                                                                                                                                               //Okuma islemleri => Async
                                                                                                                                               //Yazma islemleri => sync
                                                                                                                                               //olacak �ekilde benimsenir.
} 