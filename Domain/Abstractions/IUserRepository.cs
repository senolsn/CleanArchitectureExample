using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Insert(User user); //Insert isleminde bir db islemi olmadigi icin bunu Task ("Asenkron") olarak tanýmlamak gereksizdir. Repository Pattern'de genellikle;
                                                                                                                                               //Okuma islemleri => Async
                                                                                                                                               //Yazma islemleri => sync
                                                                                                                                               //olacak þekilde benimsenir.
} 