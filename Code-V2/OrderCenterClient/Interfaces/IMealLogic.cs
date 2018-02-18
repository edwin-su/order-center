using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterClient.Interfaces
{
    public interface IMealLogic
    {
        bool AddMeal(Meal meal);

        List<Meal> GetAllMeals();

        bool DeleteMeal(long id);

        bool ConfigureMeal(MealSchedule mealSchedule);

        List<MealSchedule> GetSchedureMeals(int mealDay);

        bool DeleteSchedule(long id);

        List<PatientArea> GetPatientAreas();

        List<Meal> GetBreakfasts();

        
    }
}
