﻿using Dapper;
using InvestmentPortfolio.Models.Models;
using InvestmentPortfolio.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace InvestmentPortfolio.Repository.Repository
{
    public class AuthRepository:IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        public async Task<bool> RegisterUser(User user)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    user.Name,
                    user.Email,
                    user.Password,
                    RoleId = 2,
                    CreatedBy = "System"
                };

                try
                {
                    await connection.ExecuteAsync("sp_RegisterUser", parameters, commandType: CommandType.StoredProcedure);
                    return true;
                }
                catch(SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> ValidateUser(string email, string passwordHash)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Email = email, Password = passwordHash };
                return await connection.ExecuteScalarAsync<bool>("sp_ValidateUser", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<dynamic>> GetFeatures()
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(
                    "[dbo].[IP_GetFeatures]",
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

    }
}
