using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using OrderCenterClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Logics
{
    public class MealLogic : IMealLogic
    {
        private readonly ICache _cache;

        public MealLogic(ICache cache)
        {
            _cache = cache;
        }

        public bool AddMeal(Meal meal)
        {
            if (string.IsNullOrWhiteSpace(meal.Name))
            {
                throw new Exception("菜名不能为空");
            }

            if (meal.Price <= 0)
            {
                throw new Exception("价格不能为空");
            }

            var sql = "INSERT INTO `order_center`.`meal` (`meal_id`,`meal_name`,`meal_description`,`price`,`is_breakfast`,`is_active`) VALUES(NULL,@name,@description,@price,@is_breakfast,1)";
            var parameters = new Parameter[]
            {
                new Parameter("@name", meal.Name),
                new Parameter("@price", meal.Price),
                new Parameter("@description", meal.Description),
                new Parameter("@is_breakfast", meal.IsBreakfast)
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        public List<Meal> GetAllMeals()
        {
            var sql = "SELECT `meal`.`meal_id`,`meal`.`meal_name`,`meal`.`meal_description`,`meal`.`price`,`meal`.`is_breakfast`,`meal`.`is_active` FROM `order_center`.`meal`; ";
            return DBHelper.ExecuteDataReader<List<Meal>>(sql, null, (reader) =>
            {
                var meals = new List<Meal>();
                while (reader.Read())
                {
                    meals.Add(new Meal()
                    {
                        Id = reader.GetInt64("meal_id"),
                        Name = reader.GetString("meal_name"),
                        Price = reader.GetDouble("price"),
                        IsBreakfast = reader.GetBoolean("is_breakfast"),
                        Description = reader.GetString("meal_description")
                    });
                }
                return meals;
            });
        }

        public bool DeleteMeal(long id)
        {
            if (id < 1)
            {
                throw new Exception("此菜品不存在");
            }

            var sql = "DELETE FROM `order_center`.`meal` WHERE meal_id=@id";
            var parameters = new Parameter[]
            {
                new Parameter("@id", id)
            };

            return DBHelper.ExecuteNonQuery(sql, parameters);
        }


        public List<MealSchedule> GetSchedureMeals(int mealDay)
        {
            if (mealDay < 0 || mealDay > 6)
            {
                throw new Exception("请输入星期一至星期日中的一天");
            }

            return _GetSchedureMeals(mealDay);
        }

        public bool ConfigureMeal(MealSchedule mealSchedule)
        {
            if (mealSchedule.MealId < 1)
            {
                throw new Exception("请输入有效的菜品！");
            }

            if (mealSchedule.MealDay < 0 || mealSchedule.MealDay > 6)
            {
                throw new Exception("请输入星期一至星期天的一天！");
            }

            if (mealSchedule.MealType < 1 || mealSchedule.MealType > 3)
            {
                throw new Exception("请输入早中晚三餐！");
            }

            return _ConfigureMeal(mealSchedule);
        }

        public List<PatientArea> GetPatientAreas()
        {
            var patientAreas = _GetActivePatientAreas();
            if (patientAreas == null || patientAreas.Count == 0)
            {
                throw new Exception("没有可选病区");
            }
            return patientAreas;
        }

        

        public List<Meal> GetBreakfasts()
        {
            return _GetBreakfasts();
        }

        private List<Meal> _GetBreakfasts()
        {
            var sql = "select * from meal where is_breakfast = 1 and is_active = 1";
            return DBHelper.ExecuteDataReader<List<Meal>>(sql, null, (reader) =>
            {
                var meals = new List<Meal>();
                while (reader.Read())
                {
                    meals.Add(new Meal()
                    {
                        Id = reader.GetInt64("meal_id"),
                        Name = reader.GetString("meal_name"),
                        Description = reader.GetString("meal_description"),
                        MealType = 1,
                        Price = reader.GetDouble("price"),
                    });
                }
                return meals;
            });
        }

        #region DB Layer
        private bool _ConfigureMeal(MealSchedule mealSchedule)
        {
            var sql = "INSERT INTO `order_center`.`meal_schedule` (`id`,`meal_day`,`meal_id`,`date_created`,`last_updated`,`meal_type`) VALUES(NULL, @meal_day,@meal_id,now(),now(),@meal_type)";
            var parameters = new Parameter[]
            {
                new Parameter("@meal_day", mealSchedule.MealDay),
                new Parameter("@meal_id", mealSchedule.MealId),
                new Parameter("@meal_type", mealSchedule.MealType)
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private List<MealSchedule> _GetSchedureMeals(int mealDay)
        {
            var sql = "SELECT s.id, m.meal_id,m.meal_name,m.meal_description,m.price, s.meal_type,s.meal_day FROM `order_center`.`meal` m INNER JOIN `order_center`.`meal_schedule` s on m.meal_id = s.meal_id WHERE m.is_active = 1 AND s.meal_day = @meal_day";
            return DBHelper.ExecuteDataReader<List<MealSchedule>>(sql, new Parameter[] { new Parameter("@meal_day", mealDay) }, (reader) =>
            {
                var meals = new List<MealSchedule>();
                while (reader.Read())
                {
                    meals.Add(new MealSchedule()
                    {
                        Id = reader.GetInt64("id"),
                        MealId = reader.GetInt64("meal_id"),
                        MealDay = reader.GetInt32("meal_day"),
                        MealType = reader.GetInt32("meal_type"),
                        Meal = new Meal()
                        {
                            Id = reader.GetInt64("meal_id"),
                            Name = reader.GetString("meal_name"),
                            Price = reader.GetDouble("price"),
                            MealType = reader.GetInt32("meal_type"),
                            Description = reader.GetString("meal_description")
                        }
                    });
                }
                return meals;
            });
        }

        public bool DeleteSchedule(long id)
        {
            if (id < 1)
            {
                throw new Exception("请输入一个有效的配菜！");
            }

            return _DeleteSchedule(id);
        }

        public bool _DeleteSchedule(long id)
        {
            var sql = "DELETE FROM `order_center`.`meal_schedule` WHERE id=@id";
            var parameters = new Parameter[]
            {
                new Parameter("@id", id)
            };

            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private List<PatientArea> _GetActivePatientAreas()
        {
            var sql = "SELECT * FROM patient_area WHERE is_active = 1";

            return DBHelper.ExecuteDataReader<List<PatientArea>>(sql, null, (reader) =>
            {
                var patientAreas = new List<PatientArea>();
                while (reader.Read())
                {
                    patientAreas.Add(new PatientArea()
                    {
                        Id = reader.GetInt32("id"),
                        AreaCode = reader.GetString("area_code"),
                    });
                }
                return patientAreas;
            });
        }

        

        


        #endregion
    }
}