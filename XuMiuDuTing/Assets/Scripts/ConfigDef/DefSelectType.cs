// ******************************************************************
//       /\ /|       @file       DefSelectType.cs
//       \ V/        @brief      excel(由python自动生成) ./xlsx//Skill.xlsx
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

namespace Rabi
{
    public static class DefSelectType
    {
        public static readonly string DAll = "All";  //全部可选
        public static readonly string DEnemy = "Enemy";  //全部敌人可选
        public static readonly string DCharacter = "Character";  //全部友方可选
        public static readonly string DAllExceptSelf = "AllExceptSelf";  //除了自己全部可选
        public static readonly string DEnemyExceptSelf = "EnemyExceptSelf";  //除了自己全部敌人可选
        public static readonly string DCharacterExceptSelf = "CharacterExceptSelf";  //除了自己全部友方可选
    }
}