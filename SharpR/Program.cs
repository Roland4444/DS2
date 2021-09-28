// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace SharpR
{
        public class Program
        {

        public Program(){
            numberBlocks = (iterations / step /blocks + 1);
            Console.WriteLine("iterations ::"+ iterations.ToString());
            Console.WriteLine("step ::"+ step.ToString());
            Console.WriteLine("blocks ::"+ blocks.ToString());
            Console.WriteLine("NUMBER BLOCKS ::"+ numberBlocks.ToString());
        }     
        public Program(String mod, int modinit, float outs){
            numberBlocks = (iterations / step /blocks + 1);
            Console.WriteLine("iterations ::"+ iterations.ToString());
            Console.WriteLine("step ::"+ step.ToString());
            Console.WriteLine("blocks ::"+ blocks.ToString());
            Console.WriteLine("NUMBER BLOCKS ::"+ numberBlocks.ToString());
        }   
        float closefirst=0.0f;
        const int delta = 300;
        string appendix = "____";
        int step = 1200;
        int iterations = 64*8640000;
        const int blocks = 2*46080;//1048576;
        int delta_mult = 10000;
        int numberBlocks;       
        String modStr = "_1";
        int modint=0;
        float outs =109.26246f;
        Boolean must_packed = true;
        public  void ProceedBlock(float[] array, int ConstantBlock, int block){
            int StartingFrom = (block-1)*ConstantBlock*step;
            int FinishedBlock = StartingFrom+ConstantBlock*step;
            Console.WriteLine("Proceed Block  #" + (block+modint).ToString());
            if (FinishedBlock >= iterations )
                FinishedBlock = iterations-1;
      
            if (FinishedBlock<StartingFrom) 
                return;
            StreamWriter open_ = new StreamWriter("open"+ (block+modint).ToString()+modStr);
            StreamWriter high_ = new StreamWriter("high"+ (block+modint).ToString()+modStr);
            StreamWriter low_ = new StreamWriter("low"+ (block+modint).ToString()+modStr);
            StreamWriter close_ = new StreamWriter("close"+ (block+modint).ToString()+modStr);           

            var counter = StartingFrom;
            while (counter + step < FinishedBlock){
                helper.proceedRange(array, counter, step, open_, low_, high_, close_);
                counter += step;

            }           
            open_.Close();
            high_.Close();
            low_.Close();
            close_.Close();
        }     
        public int calculateAll(){            
            int counter =1;
            int counter2 =1;
            string curFile = "open1_"+counter;
            while (File.Exists(curFile)){
                counter++;
                curFile = "open1_"+counter;
            }
            string curFile2 = "open2_"+counter2; 
            if  (!File.Exists(curFile2))
                return counter;
            while (File.Exists(curFile2)){
                counter2++;
                curFile2 = "open2_"+counter2;
            } 
            return counter2;

        }

        public void pack1(StreamWriter packed){
            string open__;
            StreamReader fopen = new StreamReader("open1_1");                        
            StreamReader flow = new StreamReader("low1_1");
            StreamReader fhigh = new StreamReader("high1_1");
            StreamReader fclose = new StreamReader("close1_1");
            while ((open__ = fopen.ReadLine()) != null)                        
                packed.WriteLine($"{open__}\t{flow.ReadLine()}\t{fhigh.ReadLine()}\t{fclose.ReadLine()}");
            fopen.Close();
            flow.Close();
            fhigh.Close();
            fclose.Close();                        
        }                
        

        public void deepPack(){
            int allVariants=calculateAll();
            for (int j=1;j<allVariants; j++)
            {        
                var packed = new StreamWriter(j+appendix);
                pack1(packed);
                if (File.Exists("open"+j+appendix))
                    {
                        string open__;
                        StreamReader fopen = new StreamReader("open"+j+appendix);                        
                        StreamReader flow = new StreamReader("low"+j+appendix);
                        StreamReader fhigh = new StreamReader("high"+j+appendix);
                        StreamReader fclose = new StreamReader("close"+j+appendix);
                        while ((open__ = fopen.ReadLine()) != null)
                        {
                            packed.WriteLine($"{open__}\t{flow.ReadLine()}\t{fhigh.ReadLine()}\t{fclose.ReadLine()}");
                        }
                        fopen.Close();
                        flow.Close();
                        fhigh.Close();
                        fclose.Close();                        
                    }                
                 packed.Close();
                 Console.WriteLine("Packed =>"+j+appendix);
                 }
                        
         }
               
     
        public void pack(){
            int allVariants=calculateAll();
            Console.WriteLine("allVariants {0}", allVariants);
            for (int j=1;j<allVariants; j++)
            {                
                foreach (var item in new string[]{"open", "low","high", "close"})
                {
                        var packed = new StreamWriter(item+j+appendix);
                        for (int i=1;i<10; i++)
                        {
                            string filename = item+i+"_"+j;
                            Console.WriteLine("Check {0}", filename);
                            if (File.Exists(item+i+"_"+j)){
                                Console.WriteLine("Reading ::{0}", filename);
                                foreach (string line in File.ReadLines(filename))
                                {
                                    packed.WriteLine(line);
                                }
                            }
                
                        }  
                        packed.Close();
                }

              
            }
                     
        }
        public static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Program pr = new Program();
            var outs___ = new List<float>();
            float[] outs___a = new float[pr.iterations];;
            Random rnd = new Random();
            float outs = pr.outs;
            for (int i=0; i<pr.iterations; i++) 
            {
                if (rnd.NextDouble() < 0.5) {
                    if (rnd.NextDouble() < 0.5) outs = outs - (float)rnd.NextDouble() / delta;
                    if (rnd.NextDouble() > 0.5) outs = outs + (float)rnd.NextDouble() / delta;  //adding
                }
                if (rnd.NextDouble() > 0.5) {
                    if (rnd.NextDouble() < 0.5) outs = outs * (1 + (float)rnd.NextDouble() / pr.delta_mult);
                    if (rnd.NextDouble() > 0.5) outs = outs * (1 - (float)rnd.NextDouble() / pr.delta_mult); //multyple
                }
                outs___a[i]=(outs);
            }         
            Console.WriteLine("Loop success");           
            Console.WriteLine(pr.numberBlocks);
            for (int i = 1; i<=pr.numberBlocks; i++)
                pr.ProceedBlock(outs___a, blocks, i);
            if (pr.must_packed){
                pr.pack();
                pr.deepPack();
            }
            watch.Stop();            
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds/1000} s");                        
        }
    }    
        static class helper{
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static float proceedRange(float[] Arr,  int StartIndex,int step__, params StreamWriter[] args )
        {
            var subarray = SubArray(Arr, StartIndex, step__);
            if (subarray.Length==0) return 0;        
            args[0].WriteLine(subarray[0]);
            args[1].WriteLine(subarray.Min());
            args[2].WriteLine(subarray.Max());
            args[3].WriteLine(subarray[step__ -1]);
            return subarray[step__ -1];
        }
        }

}