using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;

namespace TimetableServer.HelperClasses;

public static class LessonModelRequestExtentions
{
    public static Lesson ParseToDbModel(this LessonRequestForm lesson, Teacher teacherFromDb)
    {
        return new Lesson(){
            Teacher = teacherFromDb,
            Room = lesson.Room,
            ClassName = lesson.ClassName,
            Start = lesson.Start,
            Finish = lesson.Finish,
            LessonNumber = lesson.LessonNumber
        };;
    }
}
