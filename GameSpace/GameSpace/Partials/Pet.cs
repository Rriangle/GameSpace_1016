using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models;

public partial class Pet
{
    /// <summary>
    /// 名稱別名（向後相容）
    /// </summary>
    [NotMapped]
    public string Name
    {
        get => PetName;
        set => PetName = value;
    }

    /// <summary>
    /// 背景別名（向後相容）
    /// </summary>
    [NotMapped]
    public string Background
    {
        get => BackgroundColor;
        set => BackgroundColor = value;
    }

    /// <summary>
    /// 是否啟用（預設為 true）
    /// </summary>
    [NotMapped]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 當前經驗值別名（向後相容）
    /// </summary>
    [NotMapped]
    public int CurrentExperience
    {
        get => Experience;
        set => Experience = value;
    }

    /// <summary>
    /// 升到下一級所需經驗值（計算屬性）
    /// 根據 MiniGame_Area_完整描述文件.md 第 508-513 行規定：
    /// Level 1–10: EXP = 40 × level + 60
    /// Level 11–100: EXP = 0.8 × level² + 380
    /// Level ≥ 101: EXP = 285.69 × (1.06^level)
    /// </summary>
    [NotMapped]
    public int ExperienceToNextLevel
    {
        get
        {
            if (Level >= 1 && Level <= 10)
            {
                // Level 1-10: EXP = 40 × level + 60
                return 40 * Level + 60;
            }
            else if (Level >= 11 && Level <= 100)
            {
                // Level 11-100: EXP = 0.8 × level² + 380
                return (int)(0.8 * Level * Level + 380);
            }
            else // Level >= 101
            {
                // Level ≥ 101: EXP = 285.69 × (1.06^level)
                return (int)(285.69 * Math.Pow(1.06, Level));
            }
        }
    }
}
