using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.Images;

/// <summary>
/// Метаданные об объекте
/// </summary>
public class MataData
{
    public string Name = string.Empty;
}

/// <summary>
/// Класс для хранения данных о образе
/// </summary>
public class ComponentData
{
    // Данные об образе
    public MataData Data;
    // перцептивный хэш
    public ulong PerceptualHash;

    public ComponentData(MataData data)
    {
        Data = data;
    }

    public ComponentData()
    {
        Data = new MataData();
    }
}
