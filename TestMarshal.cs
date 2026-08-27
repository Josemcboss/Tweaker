using System;
using System.Runtime.InteropServices;

class Program {
    static void Main() {
        try {
            int[] mouseParams = new int[3] { 0, 0, 0 };
            Console.WriteLine(Marshal.SizeOf(mouseParams));
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
