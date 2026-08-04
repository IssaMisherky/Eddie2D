using System.Diagnostics.Tracing;
using UnityEngine;


public class Arc
{
    private bool _isClockwise = true;

    public float[][] ranges;


    public Arc()
    {
        ranges = new float[0][];
    }


    public int getEdgeIndex(float point)
    {
        // TODO: This algorythm should start from center going out (Binary search) /

        // TODO: Support not found index (Empty ranges)
        int index = 1;

        int rangesCount = ranges.Length * 2;

        int i = 1;

        // TODO: Change to for until count in order to prevent endless loop and in case not at end/beginning edge then return -1
        while (Mathf.Abs(i) < rangesCount)
        {
            int iPrev       = (i - 1 + rangesCount) % rangesCount;
            int iCurrent    = (i + rangesCount) % rangesCount;

            float prev = ranges[iPrev / 2][iPrev % 2];
            float current = ranges[iCurrent / 2][iCurrent % 2];

            if (prev < current)
            {
                // TODO: In case in range then check <>= otherwise <>
                if (point >= prev && point <= current)
                {
                    break;
                }
            }
            else
            {
                // TODO: In case in range then check <>= otherwise <>
                // Handles edges where it is flipped to complete the loop
                if (point > prev ||  point < current)
                {
                    break;
                }
            }

            if (point <= prev)
            {
                i--;
            }
            else
            {
                i++;
            }

            index = i;
        }

        return index;
    }


    public void addRange(float start, float end)
    {
        if (ranges.Length == 0)
        {
            ranges = new float[][] {
                new float[] { start, end }
            };
            return;
        }

        int rangesCount = ranges.Length * 2;

        int startIndex  = getEdgeIndex(start);
        int endIndex    = getEdgeIndex(end);

        bool startIsCut = startIndex % 2 == 1;
        bool endIsCut   = endIndex % 2 == 1;

        if (start > end)
        {
            //if (endIndex == 0 && startIndex == rangesCount)
            //{
            //    endIndex = rangesCount;
            //}

            if (startIndex == endIndex)
            {
                ranges = new float[][] { new float[] { start, end } };
                return;
            }
        }

        int removeCount;

        int startCut;
        int endCut;


        if (start > end)
        {
            startCut    = startIndex / 2;
            endCut      = (endIndex + 1) / 2;

            int count = (rangesCount - startIndex + 1) / 2 + (endIndex + 1) / 2;

            removeCount = count - 1;
        }
        else
        {
            startCut = startIndex / 2;
            endCut = (endIndex + 1) / 2;

            removeCount = endCut - startCut - 1;
        }

        float[][] newRanges = new float[ranges.Length - removeCount][];

        if (start <= end)
        {
            for (int i = 0; i < newRanges.Length; i++)
            {
                if (i < startCut)
                {
                    newRanges[i] = ranges[i];
                }
                else if (i == startCut)
                {
                    newRanges[i] = new float[2];
                }
                else
                {
                    newRanges[i] = ranges[i + removeCount];
                }
            }

            newRanges[startCut][0] = startIsCut ? ranges[startCut][0] : start;
            newRanges[startCut][1] = endIsCut ? ranges[endIndex / 2][1] : end;
        }
        else
        {
            for (int i = 0; i < newRanges.Length; i++)
            {
                if (endCut + i < ranges.Length)
                {
                    newRanges[i] = ranges[endCut + i];
                }
                else
                {
                    newRanges[i] = new float[2];
                }
            }

            if (!startIsCut && !endIsCut)
            {
                newRanges[startCut - endCut][0] = start;
                newRanges[startCut - endCut][1] = end;
            }
            else if (startIsCut && endIsCut)
            {
                // TODO: Make a test for this case
                newRanges[startCut - endCut][0] = ranges[startCut][0];
                newRanges[startCut - endCut][1] = ranges[endIndex / 2][1];
            }
            else
            {
                if (startIsCut)
                {
                    newRanges[startCut - endCut][0] = ranges[startCut][0];
                    newRanges[startCut - endCut][1] = end;
                }
                if (endIsCut)
                {
                    newRanges[startCut - endCut][0] = start;
                    newRanges[startCut - endCut][1] = ranges[endIndex / 2][1];
                }
            }
        }

        ranges = newRanges;
    }

    public void removeRange(float start, float end)
    {
        if (ranges.Length == 0)
        {
            return;
        }

        int startIndex = getEdgeIndex(start) % (ranges.Length * 2);
        int endIndex = getEdgeIndex(end) % (ranges.Length * 2);

        bool startIsCut = startIndex % 2 == 1;
        bool endIsCut = endIndex % 2 == 1;

        int startCut = startIndex / 2;
        int endCut = endIndex / 2;

        int removeCount = (startIndex - endIndex - 2) / 2;


    }

    public float pointScan(float start, bool clockwise)
    {
        if (ranges.Length == 0)
        {
            return -1;
        }

        int edgeIndex = getEdgeIndex(start);

        if (edgeIndex % 2 == 1)
        {
            return start;
        }

        int count = ranges.Length * 2;

        if (clockwise)
        {
            edgeIndex %= count;
        }
        else
        {
            edgeIndex = (edgeIndex - 1 + count) % count;
        }

        return ranges[edgeIndex / 2][edgeIndex % 2];
    }
}