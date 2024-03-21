using LINQSI3.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LINQSI3.Repository
{

    /*
     * This is a generic class for crud operations on databases
     * the model names must be the same as the tables from the database
     * **/

    public class Repository<T>
        where T : class
    {
        private string _connection;

        public Repository(string connection)
        {
            _connection = connection;
        }

        public List<T> FindAll()
        {
            List<T> entities = new List<T>();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                string query = "SELECT * FROM " + typeof(T).Name;
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    T t = Activator.CreateInstance<T>();

                    PropertyInfo[] properties = typeof(T).GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        if (reader[property.Name] != DBNull.Value)
                        {
                            property.SetValue(t, Convert.ChangeType(reader[property.Name], property.PropertyType));
                        }
                    }

                    entities.Add(t);
                }

                reader.Close();
            }

            return entities;
        }

        public T FindById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                string query = $"SELECT * FROM {typeof(T).Name} WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    T t = Activator.CreateInstance<T>();
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (reader[property.Name] != DBNull.Value)
                        {
                            property.SetValue(t, Convert.ChangeType(reader[property.Name], property.PropertyType));
                        }
                    }

                    reader.Close();
                    return t;
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
        }

        public void Insert(T t)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                string columns = string.Join(",", properties.Select(p => p.Name).Where(p => !p.Equals("Id")));
                string values = string.Join(",", properties.Select(p => "@" + p.Name).Where(p => !p.Equals("@Id")));
                string query = $"INSERT INTO {typeof(T).Name} ({columns}) VALUES ({values})";

                SqlCommand command = new SqlCommand(query, connection);

                foreach (PropertyInfo property in properties)
                {
                    command.Parameters.AddWithValue("@" + property.Name, property.GetValue(t));
                }

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Update(T t)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                string columnsToUpdate = string.Join(",", properties.Where(p => p.Name != "Id").Select(p => $"{p.Name} = @{p.Name}"));
                string query = $"UPDATE {typeof(T).Name} SET {columnsToUpdate} WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);

                foreach (PropertyInfo property in properties.Where(p => p.Name != "Id"))
                {
                    command.Parameters.AddWithValue("@" + property.Name, property.GetValue(t));
                }

                PropertyInfo idProperty = typeof(T).GetProperty("Id");
                command.Parameters.AddWithValue("@Id", idProperty.GetValue(t));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Delete(T t)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                PropertyInfo idProperty = typeof(T).GetProperty("Id");

                if (idProperty == null)
                {
                    throw new ArgumentException("The type T must have an 'Id' property for deletion.");
                }

                string query = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", idProperty.GetValue(t));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                string query = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
