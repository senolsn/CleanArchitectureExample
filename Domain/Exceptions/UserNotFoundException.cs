using Domain.Exceptions.Base;
using System;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string email) 
        : base($"Kullanýcý bulunamadý : {email}")
    {
    }
}