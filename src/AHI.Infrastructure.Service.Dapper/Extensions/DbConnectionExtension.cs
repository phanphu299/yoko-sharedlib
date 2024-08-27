using System;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using System.Threading.Tasks;
using System.Data;
using AHI.Infrastructure.Service.Dapper.Model;
using System.Dynamic;
using Dapper;

namespace AHI.Infrastructure.Service.Dapper.Extension
{
    public static class DbConnectionExtension
    {
        public static async Task<IEnumerable<T>> GetDataAsync<T>(this IDbConnection dbConnection, IQueryService queryService, QueryCriteria queryCriteria, string SQLscript)
        {
            using var connection = dbConnection;
            try
            {
                (string query, dynamic parameters) = queryService.CompileQuery($@"SELECT * FROM ({SQLscript}) a", queryCriteria);
                return (await connection.QueryAsync<T>(query.ToLower(), parameters as ExpandoObject)).AsList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static async Task<IEnumerable<T>> GetDataAsync<TFirst, TSecond, T>(this IDbConnection dbConnection, IQueryService queryService, QueryCriteria queryCriteria, string SQLscript, Func<TFirst, TSecond, T> map, string splitOn)
        {
            using var connection = dbConnection;
            try
            {
                (string query, dynamic parameters) = queryService.CompileQuery($@"SELECT * FROM ({SQLscript}) a", queryCriteria);
                return (await connection.QueryAsync<TFirst, TSecond, T>(query.ToLower(), map, parameters as ExpandoObject, splitOn: splitOn)).AsList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static async Task<IEnumerable<T>> GetDataAsync<TFirst, TSecond, TThird, T>(this IDbConnection dbConnection, IQueryService queryService, QueryCriteria queryCriteria, string SQLscript, Func<TFirst, TSecond, TThird, T> map, string splitOn)
        {
            using var connection = dbConnection;
            try
            {
                (string query, dynamic parameters) = queryService.CompileQuery($@"SELECT * FROM ({SQLscript}) a", queryCriteria);
                return (await connection.QueryAsync<TFirst, TSecond, TThird, T>(query.ToLower(), map, parameters as ExpandoObject, splitOn: splitOn)).AsList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static async Task<IEnumerable<T>> GetDataAsync<TFirst, TSecond, TThird, TFourth, T>(this IDbConnection dbConnection, IQueryService queryService, QueryCriteria queryCriteria, string SQLscript, Func<TFirst, TSecond, TThird, TFourth, T> map, string splitOn)
        {
            using var connection = dbConnection;
            try
            {
                (string query, dynamic parameters) = queryService.CompileQuery($@"SELECT * FROM ({SQLscript}) a", queryCriteria);
                return (await connection.QueryAsync<TFirst, TSecond, TThird, TFourth, T>(query.ToLower(), map, parameters as ExpandoObject, splitOn: splitOn)).AsList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static async Task<IEnumerable<T>> GetDataAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, T>(this IDbConnection dbConnection, string query, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, T> map, object param, string splitOn)
        {
            using var connection = dbConnection;
            try
            {
                return (await connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, T>(query.ToLower(), map, param, splitOn: splitOn)).AsList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static async Task<int> CountDataAsync(this IDbConnection dbConnection, IQueryService queryService, QueryCriteria queryCriteria, string SQLscript)
        {
            using var connection = dbConnection;
            try
            {
                queryCriteria.Sorts = null;
                (string query, dynamic parameters) = queryService.CompileQuery(
                    $@"SELECT COUNT(*) FROM ({SQLscript}) a",
                    queryCriteria, paging: false);
                return await connection.ExecuteScalarAsync<int>(query.ToLower(), parameters as ExpandoObject);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}