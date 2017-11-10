// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using UnityEditor;
using UnityEngine;

namespace Uplift
{
    public class UpliftPreferences : MonoBehaviour
    {
        private static bool prefsLoaded = false;
        private static readonly string useExperimentalFeaturesKey = "UpliftExperimentalFeatures";
        private static readonly string trustUnknownCertificatesKey = "UpliftUnknownCertificates";

        private static bool useExperimentalFeatures;
        private static bool trustUnknownCertificates;

        [PreferenceItem("Uplift")]
        public static void PreferencesGUI()
        {
            if(!prefsLoaded)
            {
                useExperimentalFeatures = EditorPrefs.GetBool(useExperimentalFeaturesKey, false);
                trustUnknownCertificates = EditorPrefs.GetBool(trustUnknownCertificatesKey, false);
                prefsLoaded = true;
            }
            EditorGUILayout.LabelField("SSL Certificates:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Unknown certificates could be the result of our registered certificates being outdated or the result of a potential attack. Trusting unknown certificates could lead to security breaches. Use at your own risk!",
                MessageType.Warning
            );
            trustUnknownCertificates = EditorGUILayout.Toggle("Trust unknown certificates", trustUnknownCertificates);

            EditorGUILayout.LabelField("Experimental features:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Experimental features are not thoroughly tested and could induce bugs. Use at your own risk!",
                MessageType.Warning
            );
            useExperimentalFeatures = EditorGUILayout.Toggle("Use experimental features", useExperimentalFeatures);

            if(GUI.changed)
            {
                EditorPrefs.SetBool(useExperimentalFeaturesKey, useExperimentalFeatures);
                EditorPrefs.SetBool(trustUnknownCertificatesKey, trustUnknownCertificates);
            }
        }

        public static bool UseExperimental()
        {
            return EditorPrefs.GetBool(useExperimentalFeaturesKey, false);
        }

        public static bool TrustUnknownCertificates()
        {
            return EditorPrefs.GetBool(trustUnknownCertificatesKey, false);
        }
    }
}
