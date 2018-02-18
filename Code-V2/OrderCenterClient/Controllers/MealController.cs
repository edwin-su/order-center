using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class MealController : Controller
    {
        private readonly IMealLogic _mealLogic;

        public MealController(IMealLogic mealLogic)
        {
            _mealLogic = mealLogic;
        }

        // GET: Meal
        public ActionResult AddMeal(Meal meal)
        {
            _mealLogic.AddMeal(meal);
            return Json(true);
        }

        public ActionResult GetMeals()
        {
            var meals = _mealLogic.GetAllMeals();

            return Json(meals);
        }

        public ActionResult DeleteMeal(long id)
        {
            return Json(_mealLogic.DeleteMeal(id));
        }

        public ActionResult ConfigureMeal(MealSchedule mealSchedule)
        {
            mealSchedule.MealDay = mealSchedule.MealDay == 7 ? 0 : mealSchedule.MealDay;
            var result = _mealLogic.ConfigureMeal(mealSchedule);
            return Json(result);
        }

        public ActionResult GetSchedureMeals(int mealDay)
        {
            mealDay = mealDay == 7 ? 0 : mealDay;
            var result = _mealLogic.GetSchedureMeals(mealDay);
            return Json(result);
        }

        public ActionResult DeleteSchedule(long id)
        {
            var result = _mealLogic.DeleteSchedule(id);

            return Json(result);
        }

        public ActionResult GetMealsAndAreas(int mealDay)
        {
            mealDay = mealDay == 7 ? 0 : mealDay;
            List<Meal> breakfastMeals = new List<Meal>();
            List <Meal> lunchMeals = new List<Meal>();
            List<Meal> dinnerMeals = new List<Meal>();

            var shedules = _mealLogic.GetSchedureMeals(mealDay);

            breakfastMeals = _mealLogic.GetBreakfasts();
            lunchMeals = shedules.Where(x => x.MealType == 2).Select(x => x.Meal).ToList();
            dinnerMeals = shedules.Where(x => x.MealType == 3).Select(x => x.Meal).ToList();

            List <PatientArea> patientAreas = _mealLogic.GetPatientAreas();

            return Json(new { BreakfastMeals = breakfastMeals, LunchMeals = lunchMeals, DinnerMeals = dinnerMeals, PatientAreas = patientAreas });
        }

        
    }
}