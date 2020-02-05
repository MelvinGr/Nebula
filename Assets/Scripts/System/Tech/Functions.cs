using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

public class Functions : MonoBehaviour
{
	public static Vector3 Direction(Vector3 from, Vector3 to)
	{
		return (to - from).normalized;
	}
	
	public static Vector3 RelativePos(Vector3 from, Vector3 to)
	{
		return (to - from);
	}

	public static int DistanceFloored(Vector3 one, Vector3 two)
	{
		return Mathf.FloorToInt(Vector3.Distance(one, two));
	}
	
	public static float Angle(Vector3 from, Vector3 target, Vector3 forward)
	{
	    Vector3 targetDir = target - from;
        //Vector3 forward = from.forward;
        return Vector3.Angle(targetDir, forward);
	}
	
	public static void MergeDictionary<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
	{
		if (second == null) 
			return;
		
		if (first == null) 
			first = new Dictionary<TKey, TValue>();
		
		foreach (var item in second) 
		{
			if (!first.ContainsKey(item.Key)) 
				first.Add(item.Key, item.Value);
		}
	}
	
	public static Dictionary<string, Transform> GetChildTransformDictionary(Transform transform, bool recruisive)
	{
		Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
		foreach(Transform child in transform)
		{			
			dict.Add(child.name, child);
			
			if(recruisive)
				MergeDictionary(dict, GetChildTransformDictionary(child, recruisive));
		}
		
		return dict;
	}

	public static List<Transform> FindChildTransformsRecruisive(Transform transform, string name, bool sort)
	{
		List<Transform> list = new List<Transform>();
		foreach(Transform child in transform)
		{			
			if(child.name.ToLower().Trim().IndexOf(name.ToLower().Trim()) >= 0)
				list.Add(child);
			else
				list.AddRange(FindChildTransformsRecruisive(child, name, sort));
		}

		return (sort ? SortList(list) : list);
	}

	public static T GetComponentInGameObject<T>(string gameObject)
	{
		GameObject pObject = GameObject.Find(gameObject);
		if(pObject != null)
			return (T)System.Convert.ChangeType(pObject.GetComponent(typeof(T).ToString()), typeof(T));

		return default(T);
	}
	
	public static Vector3 ClampVector3(Vector3 value, Vector3 min, Vector3 max)
	{
		float x, y, z;

		if(value.x < min.x)
			x = min.x;
		else if(value.x > max.x)
			x = max.x;
		else 
			x = value.x;
			
		if(value.y < min.y)
			y = min.y;
		else if(value.y > max.y)
			y = max.y;
		else 
			y = value.y;
			
		if(value.z < min.z)
			z = min.z;
		else if(value.z > max.z)
			z = max.z;
		else 
			z = value.z;
			
		return new Vector3(x, y, z);
	}
	
	public static bool RectangleContains(Rect rect, Vector3 vVec)
	{
		return RectangleContains(rect, new Vector2(vVec.x, vVec.z));
	}
	
	public static bool RectangleContains(Rect rect, Vector2 vVec)
	{			
		return 
			(vVec.x >= rect.xMin) &&
			(vVec.x <= rect.xMin + rect.width) &&
			(vVec.y >= rect.yMin) &&
			(vVec.y <= rect.yMin + rect.height);
	}

	public static Vector2[] RectToVector2(Rect rect)
	{
		Vector2[] tmp = 
		{ 			
			new Vector2(rect.xMin, rect.yMin), // top left
			new Vector2(rect.width, rect.yMin),  // top right
			
			new Vector2(rect.width, rect.height), // bottom right
			new Vector2(rect.xMin, rect.height) // bottom left
		};
		
		return tmp;
	}
	
	public static Vector3[] RectToVector3(Rect rect, float height)
	{
		Vector3[] tmp = 
		{ 			
			new Vector3(rect.xMin, height, rect.yMin), // top left
			new Vector3(rect.width, height, rect.yMin),  // top right
			
			new Vector3(rect.width, height, rect.height), // bottom right
			new Vector3(rect.xMin, height, rect.height) // bottom left
		};
		
		return tmp;
	}
	
	public static Vector2 RectCenter(Rect rect)
	{
		Vector2[] vects = RectToVector2(rect);		
		return new Vector2
		(
			vects[0].x + (vects[2].x - vects[0].x) / 2, 
			vects[0].y + (vects[0].y - vects[2].x) / 2
		);
	}
	
	public static Vector3 Vector2To3(Vector2 vect, float height = 0)
	{
		return new Vector3(vect.x, height, vect.y);
	}
	
	public static Vector3 ScreenCenterInWorldSpace(SceneCamera camera, float height = 0)
	{
		return Vector2To3(RectCenter(camera.Constrains), height);
	}
	
	public static void VisualizeRect(Rect rect, Color color, float height = 0)//params Color[] colors)
	{
		Vector3[] vects = RectToVector3(rect, height);
		
		Debug.DrawLine(vects[0], vects[1], color);  
		Debug.DrawLine(vects[1], vects[2], color); 
		Debug.DrawLine(vects[2], vects[3], color);  
		Debug.DrawLine(vects[3], vects[0], color);  		
	}
	
	/*public static List<string> FileToList(string file, bool ignoreComments)
	{
		TextAsset levelFile = (TextAsset)Resources.Load(file);		
		List<string> blockArrangement = new List<string>();
		
		string line;
		StringReader sr = new StringReader(levelFile.text);
		while ((line = sr.ReadLine()) != null)
		{
			if((!line.StartsWith("//") && line != "") && ignoreComments)
				blockArrangement.Add(line);
		}
		
		sr.Close();
		
		return blockArrangement;
	}*/
	
	public static Dictionary<string, string> FileToDictionary(string file)
	{
		TextAsset languageFile = (TextAsset)Resources.Load(file);	
		Dictionary<string, string> translatedDict = new Dictionary<string, string>();		

		if(!languageFile)
			return null;
			
		string line;
		int lineNumer = 0;
		StringReader sr = new StringReader(languageFile.text);
		while ((line = sr.ReadLine()) != null)
		{
			if(!line.StartsWith("//") && line != "")
			{				
				string[] values = line.Split('=');				
				if(values.Length != 2)
					throw new System.InvalidOperationException("File: \"" + file + ".txt\" Line: " + lineNumer + " is Incorrect!");
				
				if(translatedDict.ContainsKey(values[0]))
					throw new System.InvalidOperationException("File: \"" + file + ".txt\" Key: " + values[0] + " Exists!");
				else
					translatedDict.Add(values[0], values[1]);
			}
			
			lineNumer++;
		}
		
		sr.Close();		
		return translatedDict;
	}
	
	public static byte[] GetBytesFromFile(string fullFilePath)
	{
		FileStream fs = File.OpenRead(fullFilePath);
		try
		{
			byte[] bytes = new byte[fs.Length];
			fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
			fs.Close();
			return bytes;
		}
		finally
		{
			fs.Close();
		}
	}

	public static T DeserializeFromResourcesXML<T>(string readFrom)
	{
		TextAsset asset = (TextAsset)Resources.Load(readFrom);
		if(asset != null)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(T));		
			StringReader textReader = new StringReader(asset.text);	
			
			T obj = (T)deserializer.Deserialize(textReader);
			textReader.Close();

			return obj;
		}
		
		return default(T);
	}
	
	public static T DeserializeFromXML<T>(string xmlString)
	{	
		if(xmlString == null || xmlString == "")
			return default(T);
			
		StringReader textReader = new StringReader(xmlString);	
		XmlSerializer deserializer = new XmlSerializer(typeof(T));		
		
		T obj = default(T);		
		try
		{
			obj = (T)deserializer.Deserialize(textReader);
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
		}
		
		textReader.Close();
		return obj;
	}

	public static string SerializeToXML<T>(T obj)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		StringWriter textWriter = new StringWriter();
		serializer.Serialize(textWriter, obj);
		textWriter.Close();
		
		return textWriter.ToString();
	}
	
	public static T DeserializeFromXMLFile<T>(string readFrom)
	{
		T obj = default(T);
		
		try
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(T));		
			MemoryStream textReader = new MemoryStream(GetBytesFromFile(readFrom));	 	
			
			obj = (T)deserializer.Deserialize(textReader);
			textReader.Close();
		}
		catch(Exception e)
		{
			Console.Write(e.ToString());
		}
		
		return obj;
	}

	public static void SerializeToXMLFile<T>(T obj, string writeTo)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		TextWriter textWriter = new StreamWriter(writeTo);
		serializer.Serialize(textWriter, obj);
		textWriter.Close();
	}

	static GameObject audioPlayers;
	public static AudioSource PlayAudioClip(Vector3 position, AudioClip clip)
	{
		if(audioPlayers == null)
		{
			audioPlayers = new GameObject("AudioPlayers");
			audioPlayers.transform.position = Vector3.zero;
		}
		
		GameObject audioObject = new GameObject(clip.name + position);
		audioObject.transform.parent = audioPlayers.transform;
		audioObject.tag = "Destroyable";
		audioObject.transform.position = position;
		
		AudioSource audioSource = audioObject.AddComponent<AudioSource>();
		audioObject.AddComponent<AdjustSoundEffectLevel>();	
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.PlayOneShot(clip);
		
		return audioSource;
	}
	
	public static IDictionary<String, Int32> EnumToDictionary<K>()
	{
		if (typeof(K).BaseType != typeof(Enum))
			throw new InvalidCastException();

		return Enum.GetValues(typeof(K)).Cast<Int32>().ToDictionary(currentItem => Enum.GetName(typeof(K), currentItem));
	}
	
	public static List<T> SortList<T>(List<T> list)
	{
		list.Sort
		(
			delegate(T c1, T c2) 
			{ 
				return c1.ToString().CompareTo(c2.ToString()); 
			}
		);
		
		return list;
	}
	
	public static Vector3 MultiplyVector3(Vector3 one, Vector3 two)
	{
		return new Vector3(one.x * two.x, one.y * two.y, one.z * two.z);
	}
	
	public static string SplitCamelCase(string input)
    {
        return Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }
	
	public static Rect AddRect(Rect one, Rect two)
	{
		return new Rect(one.xMin + two.xMin, one.yMin + two.yMin, one.width + two.width, one.height + two.height);
	}
}