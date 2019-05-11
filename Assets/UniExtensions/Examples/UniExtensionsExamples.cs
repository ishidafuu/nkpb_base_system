using UnityEngine;
using System.Collections;

using UniExtensions.Async;
using UniExtensions;

public class UniExtensionsExamples : MonoBehaviour {


    [ContextMenu("MD5 Test")]
    void MD5Test() {

        var msg = System.Text.ASCIIEncoding.ASCII.GetBytes("Xyzzy");
        var tmd5 = new UniExtensions.Crypto.MD5();

        var thash = tmd5.HexDigest(msg);

        Debug.Log(thash);

        var md5 = System.Security.Cryptography.MD5.Create ();
        var hash = md5.ComputeHash (msg);
        var hex = "";
        for (var i = 0; i < hash.Length; i++) {
            hex += System.Convert.ToString (hash [i], 16).PadLeft (2, '0');
        }
        Debug.Log (hex.PadLeft (32, '0'));
    }

    //How to use Markov Generator
    //--------------------------------------------------------------
    [ContextMenu("Generate Names")]
    void GenerateNames() {
        //note, usually a much large sample set of words is used.
        var names = "Boris Vlad Kierov Alexi Yuri Artur Vasily Vladislav Viktor Sergei".Split(' ');
        var mg = new UniExtensions.MarkovGenerator(names, 2);
        Debug.Log(mg.NextString());
        Debug.Log(mg.NextString());
        Debug.Log(mg.NextString());
        Debug.Log(mg.NextString());
        Debug.Log(mg.NextString());
    }

	//How to use ExtCoroutine
	//--------------------------------------------------------------
	void ExtCoroutineDemo() {
		var task = new ExtCoroutine(ThisIsACoroutine());
		task.Start();
		task.Suspend();
		task.Abort();
	}

	IEnumerator ThisIsACoroutine() {
		while(true) {
			yield return null;
		}
	}


	//How to use MagicThreads
	//--------------------------------------------------------------
	void MagicThreadDemo() {
		MagicThread.Start(ThisIsAMagicThread(), false);
	}

	IEnumerator ThisIsAMagicThread() {
		//at this line the coroutine is running in the main unity thread.
		yield return null;
		//the next line will move the coroutine into a background thread.
		yield return new BackgroundTask();

		//at this line the coroutine is running in a background thread.
		yield return null;

		//the next line will move the coroutine back into the main unity thread
		yield return new ForegroundTask();

		//at this line the coroutine is back running in the main unity thread.
		yield return null;
	}


    [ContextMenu("JSON Test")]
    void JsonTester() {
        var instance = new JsonTest();
        try {
            var json = UniExtensions.Serialization.JsonSerializer.Encode(instance);
            Debug.Log(json);
            Debug.Log (UniExtensions.Serialization.JsonPrettyPrint.Format(json));
        } catch(System.Exception e) {
            Debug.LogError(e);
        }

    }




    public class JsonTest {
        public string s = "Xyzzy";
        public char c = 'C';
        public int i = 42;
        public float f = 3.14f;
        public int[] intArray = new int[] { 1,2,3,4,5 };
        public string[] strArray = new string[] { "Xyzzy", "Plugh" };

    }

}
