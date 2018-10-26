
namespace WordJourney
{
	/// <summary>
	/// 日记模型
	/// </summary>
	[System.Serializable]
	public class DiaryModel
	{
		//触发关卡等级
		public int triggeredLevel;
		//英文日记
		public string diaryEN;
		//中文日记
		public string diaryCH;
		//构造方法
		public DiaryModel(int triggeredLevel, string diaryEN, string diaryCH)
		{
			this.triggeredLevel = triggeredLevel;
			this.diaryEN = diaryEN;
			this.diaryCH = diaryCH;
		}
	}
}
