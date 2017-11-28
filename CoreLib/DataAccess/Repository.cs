using CoreLib.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.DataAccess
{
    public interface IRepository<T> where T: class
    {
        DataResult<IList<T>> GetAll();
        DataResult<T> Get(int id);
        MainResult Insert(T item);
        MainResult Update(T item);
        MainResult Delete(T item);
        MainResult Delete(int id);
    }
    public class Repository<T> : IRepository<T>,IDisposable where T: class
    {
        private DataContext _ctx;
        private DbSet<T> _table;

        public Repository()
        {
            _ctx = new DataContext();
            _table = _ctx.Set<T>();
        }

        public Repository(DataContext ctx)
        {
            _ctx = ctx;
            _table = _ctx.Set<T>();
        }

        public DataResult<IList<T>> GetAll()
        {
            bool success = true;
            string message = "";
            List<T> list = null;
            try
            {
                list = _table.ToList();
            }
            catch(Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
                list = null;
            }
            return new DataResult<IList<T>>
            {
                Data = list,
                Status = new MainResult
                {
                    Success = success,
                    ErrorMessage = message
                }
            };
        }

        public DataResult<T> Get(int id)
        {
            bool success = true;
            string message = "";
            T result = null;
            try
            {
                result = _table.Find(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
                result = null;
            }
            return new DataResult<T>
            {
                Data = result,
                Status = new MainResult
                {
                    Success = success,
                    ErrorMessage = message
                }
            };
        }
        public DataResult<T> GetByFilter(Func<T,bool> filter)
        {
            bool success = true;
            string message = "";
            T result = null;
            try
            {
                //Why not FirstOrDefault()?
                //Because we have try-catch block.
                //We don't want to return success status with null result.
                result = _table.Where(filter).First(); 
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
                result = null;
            }
            return new DataResult<T>
            {
                Data = result,
                Status = new MainResult
                {
                    Success = success,
                    ErrorMessage = message
                }
            };
        }

        public MainResult Insert(T item)
        {
            bool success = true;
            string message = "";
            try
            {
                _table.Add(item);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = message
            };
        }

        public MainResult Update(T item)
        {
            bool success = true;
            string message = "";
            try
            {
                if(!_table.Local.Contains(item))
                {
                    _table.Attach(item);
                }
                _ctx.Entry(item).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = message
            };
        }

        public MainResult Delete(T item)
        {
            bool success = true;
            string message = "";
            try
            {
                _table.Remove(item);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = message
            };
        }

        public MainResult Delete(int id)
        {
            bool success = true;
            string message = "";
            try
            {
                T item = _table.Find(id);
                if(item!=null)
                {
                    _table.Remove(item);
                    _ctx.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error: `{ex.Message}`";
            }
            return new MainResult
            {
                Success = success,
                ErrorMessage = message
            };
        }

        public void Dispose()
        {
            if(_table != null)
            {
                _table = null;
            }
            if (_ctx != null)
            {
                _ctx.Dispose();
                _ctx = null;
            }
        }
    }
}
