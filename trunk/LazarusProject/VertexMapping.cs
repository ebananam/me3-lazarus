using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ME2Vector = Gibbed.MassEffect2.FileFormats.Save.Vector;
using ME3Vector = Gibbed.MassEffect3.FileFormats.Save.Vector;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Pixelmade.Lazarus
{
    [DataContract, Serializable]
    public class VertexMapping
    {
        [DataMember]
        HashSet<int>[] mapping;
        [DataMember]
        float[] thresholds;
        [DataMember]
        bool[] ignore;
        [DataMember]
        bool[] verified;
        [DataMember]
        float minThreshold = float.MaxValue;
        [DataMember]
        float maxThreshold = float.MinValue;
        [DataMember]
        float thresholdIncrement = 0.02f;

        bool modified; // Non persistent

        public HashSet<int>[] Mapping { get { return mapping; } }
        public bool[] Ignore { get { return ignore; } }
        public bool[] Verified { get { return verified; } }
        public float[] Thresholds { get { return thresholds; } }
        public float MinThreshold { get { return minThreshold; } }
        public float MaxThreshold { get { return maxThreshold; } }
        public bool Modified
        {
            get { return modified; }
            set { modified = value; }
        }

        public VertexMapping(List<ME2Vector> me2Data, List<ME3Vector> me3Data)
        {
            mapping = new HashSet<int>[me2Data.Count];
            for (int i = 0; i < mapping.Length; i++) mapping[i] = new HashSet<int>();
            thresholds = new float[mapping.Length];
            ignore = new bool[mapping.Length];
            verified = new bool[mapping.Length];

            for (int i = 0; i < me2Data.Count; i++)
            {
                ME2Vector me2Source = me2Data[i];
                Vector3 source = new Vector3(me2Source.X, me2Source.Y, me2Source.Z);
                bool matched = false;
                float threshold = 0f;

                while (!matched)
                {
                    for (int j = 0; j < me3Data.Count; j++)
                    {
                        ME3Vector me3Candidate = me3Data[j];
                        Vector3 candidate = new Vector3(me3Candidate.X, me3Candidate.Y, me3Candidate.Z);

                        if (Vector3.Distance(source, candidate) <= threshold)
                        {
                            mapping[i].Add(j);
                            matched = true;
                        }
                    }
                    threshold += thresholdIncrement;
                }
                thresholds[i] = threshold;
                if (threshold < minThreshold) minThreshold = threshold;
                if (threshold > maxThreshold) maxThreshold = threshold;

                ignore[i] = false;
                verified[i] = false;
            }
        }
    }
}
