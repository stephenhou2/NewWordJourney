
[System.Serializable]
public class DiaryModel {
	
	public int triggeredLevel;
	public string diaryEN;
	public string diaryCH;

	public DiaryModel(int triggeredLevel,string diaryEN,string diaryCH){
		this.triggeredLevel = triggeredLevel;
		this.diaryEN = diaryEN;
		this.diaryCH = diaryCH;
	}
}
