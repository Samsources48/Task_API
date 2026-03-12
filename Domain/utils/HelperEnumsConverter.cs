using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.utils
{
    public static class HelperEnumsConverter
    {
        public const string TODO = "Por hacer";
        public const string IN_PROGRESS = "En progreso";
        public const string IN_REVIEW = "En revicion";
        public const string DONE = "Hecho";

        public const string LOW = "Baja";
        public const string MEDIUM = "Media";
        public const string HIGH = "Alta";
        public const string CRITICAL = "Crítica";

        public static string statusTasksToString(this statusTasksEnum enumData)
        {
            return enumData switch
            {
                statusTasksEnum.Todo => TODO,
                statusTasksEnum.InProgress => IN_PROGRESS,
                statusTasksEnum.InReview => IN_REVIEW,
                statusTasksEnum.Done => DONE,
                _ => throw new ArgumentException(nameof(enumData), $"Not expected enum value: {enumData}"),
            };
        }

        public static statusTasksEnum stringToStatusTasksEnum(this string stringData)
        {
            return stringData switch
            {
                TODO => statusTasksEnum.Todo,
                IN_PROGRESS => statusTasksEnum.InProgress,
                IN_REVIEW => statusTasksEnum.InReview,
                DONE => statusTasksEnum.Done,
                _ => throw new ArgumentException(nameof(stringData), $"Not expected string value: {stringData}"),
            };
        }

        public static string priorityToString(this priorityEnum enumData)
        {
            return enumData switch
            {
                priorityEnum.Low => LOW,
                priorityEnum.Medium => MEDIUM,
                priorityEnum.High => HIGH,
                priorityEnum.Critical => CRITICAL,
                _ => throw new ArgumentException(nameof(enumData), $"Not expected enum value: {enumData}"),
            };
        }

        public static priorityEnum stringPriorityEnum(this string stringData)
        {
            return stringData switch
            {
                LOW => priorityEnum.Low,
                MEDIUM => priorityEnum.Medium,
                HIGH => priorityEnum.High,
                CRITICAL => priorityEnum.Critical,
                _ => throw new ArgumentException(nameof(stringData), $"Not expected string value: {stringData}"),
            };
        }
    }
}
