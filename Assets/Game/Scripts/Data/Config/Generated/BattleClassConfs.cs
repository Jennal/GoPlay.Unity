﻿// Code generated by Excel2ScriptableObject. DO NOT EDIT.
// source file: Assets/./Excels/全局设置表.xlsx
// source sheet: BattleClass@ZH_CN

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GoPlay;

namespace GoPlay.Data.Config
{
    /// <summary>
	/// BattleClassConf表单行结构
    ///
	/// 战斗职业
	/// </summary>
	[Serializable]
	public partial struct BattleClassConf
	{
		
		/// <summary>
		/// 道具品质
		/// </summary>
		public int Id;

		/// <summary>
		/// 职业名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 道具图标
		/// </summary>
		public SpriteRefer Icon;

		
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("BattleClassConf : { ");
			
			sb.AppendFormat("Id: {0}, ", Id);
			sb.AppendFormat("Name: {0}, ", Name);
			sb.AppendFormat("Icon: {0}, ", Icon);
			sb.Append(" }");
			return sb.ToString();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is BattleClassConf == false) return false;
			
			var o = (BattleClassConf)obj;
			
			if (Id != o.Id) return false;
			if (Name != o.Name) return false;
			if (Icon != o.Icon) return false;
			
			return true;
		}

        public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        public static bool operator ==(BattleClassConf lhs, BattleClassConf rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BattleClassConf lhs, BattleClassConf rhs)
		{
			return !lhs.Equals(rhs);
		}
	}

    /// <summary>
    /// BattleClassConf表结构
    ///
	/// 战斗职业
	/// </summary>
	[Serializable]
	[CreateAssetMenu(menuName="ExcelObject/BattleClassConf")]
	public class BattleClassConfs : ScriptableObject
	{	
		public List<BattleClassConf> Values = new List<BattleClassConf>(); 
	}
}
