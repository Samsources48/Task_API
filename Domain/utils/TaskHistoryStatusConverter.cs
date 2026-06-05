using Domain.Enums;

namespace Domain.utils
{
    public static class TaskHistoryStatusConverter
    {
        public static string toStringStatus(this taskHistoryStatusEnum e) => e switch
        {
            taskHistoryStatusEnum.Iniciado => "Iniciado",
            taskHistoryStatusEnum.Pausa => "Pausa",
            taskHistoryStatusEnum.Completado => "Completado",
            _ => throw new ArgumentException($"Not expected enum value: {e}")
        };

        public static taskHistoryStatusEnum toTaskHistoryStatusEnum(this string s) => s switch
        {
            "Iniciado" => taskHistoryStatusEnum.Iniciado,
            "Pausa" => taskHistoryStatusEnum.Pausa,
            "Completado" => taskHistoryStatusEnum.Completado,
            _ => throw new ArgumentException($"Not expected status string: {s}")
        };
    }
}
