using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CoreDebug {
	public static void LogCollection(String name, System.Object blob) {
		var logString = new StringBuilder ();
		var i = 0;
		var collection = (ICollection) blob;

		logString.AppendFormat ("---Begin collection: {0}", name);

		foreach (System.Object element in collection) {
			if (element is ICollection) {
				LogCollection (name, element);
			} else {
				logString.AppendFormat ("\nelement {0}: {1}", i, element.ToString ());
			}
			i++;
		}

		logString.Append ("---End collection");
		Debug.LogFormat ("{0}", logString.ToString ());
	}
}