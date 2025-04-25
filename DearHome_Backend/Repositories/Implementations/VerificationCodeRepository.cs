using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class VerificationCodeRepository : BaseRepository<VerificationCode>, IVerificationCodeRepository
{
    private new readonly DearHomeContext _context;
    public VerificationCodeRepository(DearHomeContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<VerificationCode?> GetVerificationCodeByEmailAsync(string email)
    {
        var verificationCode = await _context.VerificationCodes
            .FirstOrDefaultAsync(v => v.Email == email);
            

        return verificationCode ?? null;
    }
}
