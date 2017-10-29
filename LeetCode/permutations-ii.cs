// 47. Permutations II   
// https://leetcode.com/problems/permutations-ii
// Medium 32.1%
// 580.8115017778049
// Submission: https://leetcode.com/submissions/detail/69160145/
// Runtime: 496 ms
// Your runtime beats 29.67 % of csharp submissions.

public class Solution {
    public IList<IList<int>> PermuteUnique(int[] nums) {
            var list = new List<IList<int>>();
        Array.Sort(nums);
        bool sorted;
        do 
        {
            var clone = (int[]) nums.Clone();
            list.Add(clone);
            NextPermutation(nums);
                        sorted = true;
            for (int i=nums.Length-1; i>=1; i--)
                if (nums[i]<nums[i-1])
                {
                    sorted = false;
                    break;
                }
        }
        while (!sorted); 
        return list;
    }
        public void NextPermutation(int[] nums) {
        int len = nums.Length;
                int k;
        for (k=len-1; k>0; k--)
            if (nums[k-1]<nums[k])
                break;
                int lo=k;
        int hi=len-1;
        while (lo<hi)
            Swap(ref nums[lo++], ref nums[hi--]);
            if (k==0)
            return;
                lo=k;
        hi=len-1;
                int target = nums[k-1];
        while (lo<=hi)
        {
            int mid = lo + (hi-lo)/2;
            int v = nums[mid];
            if (target<v)
                hi = mid-1;
            else
                lo = mid+1;
        }
                Swap(ref nums[k-1], ref nums[lo]);
    }
        void Swap(ref int x, ref int y)
    {
        int tmp = x;
        x = y;
        y = tmp;
    }
}
