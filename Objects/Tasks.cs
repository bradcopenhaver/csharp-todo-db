using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Globalization;

namespace ToDoList.Objects
{
  public class Task
  {
    private string _description;
    private int _id;
    private int _category_id;
    private DateTime _due_date;

    public Task(string description, int category_id, DateTime due, int Id = 0)
    {
      _description = description;
      _id = Id;
      _category_id = category_id;
      _due_date = due;
    }

    public override bool Equals(System.Object otherTask)
    {
      if (!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool idEquality = (this.GetId() == newTask.GetId());
        bool categoryEquality = (this.GetCategoryId() == newTask.GetCategoryId());
        bool descriptionEquality = (this.GetDescription() == newTask.GetDescription());
        return (idEquality && descriptionEquality && categoryEquality);
      }
    }
    public override int GetHashCode()
    {
     return this.GetDescription().GetHashCode();
    }

    public DateTime GetDueDate()
    {
      return _due_date;
    }

    public string GetDescription()
    {
      return _description;
    }

    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public int GetCategoryId()
    {
      return _category_id;
    }

    public void SetCategoryId(int newId)
    {
      _category_id = newId;
    }
    public int GetId()
    {
      return _id;
    }
    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(2);
        int taskCategoryId = rdr.GetInt32(1);
        DateTime taskDue = rdr.GetDateTime(3);
        Task newTask = new Task(taskDescription, taskCategoryId, taskDue, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, category_id, due_date) OUTPUT INSERTED.id VALUES (@TaskDescription, @CategoryId, @DueDate);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.GetDescription();
      cmd.Parameters.Add(descriptionParameter);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetCategoryId();
      cmd.Parameters.Add(categoryIdParameter);

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@DueDate";
      dueDateParameter.Value = this.GetDueDate();
      cmd.Parameters.Add(dueDateParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Task Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
      SqlParameter taskIdParameter = new SqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = id.ToString();
      cmd.Parameters.Add(taskIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundTaskId = 0;
      int foundCategoryId = 0;
      string foundTaskDescription = null;
      DateTime foundTaskDueDate = new DateTime();
      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundCategoryId = rdr.GetInt32(1);
        foundTaskDescription = rdr.GetString(2);
        foundTaskDueDate = rdr.GetDateTime(3);
      }
      Task foundTask = new Task(foundTaskDescription, foundCategoryId, foundTaskDueDate, foundTaskId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundTask;
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
  }
}
