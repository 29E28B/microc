# 基于Microc的优化功能实现
---
- 课程名称：编程语言原理与编译
- 实验项目：期末大作业
- 专业班级：计算机1902
- 学生学号：31901053，31901051
- 学生姓名：孙国豪，舒以恒
- 实验指导教师: 张芸
-  [MyMicroc: 基于F#的C语言编译器实现。 (gitee.com)]() 
---

## 简介
这是一个编译原理大作业，主要基于老师上课发的microC完成的，优化了java虚拟机，并添加了部分未实现的功能。


## 结构
- 前端：由`F#`语言编写而成  
  - `CLex.fsl`生成的`CLex.fs`词法分析器。
  - `CPar.fsy`生成的`CPar.fs`语法分析器。
  - `Absyn.fs` 定义了抽象语法树
  - ``定义了中间表示的生成指令集(?是否有)
  - `Comp.fs`将抽象语法树转化为中间表示
- `interpc`为解释器模块
  - `microc`为编译器模块，生成中间表示
  
- 后端：由`Java`语言编写而成
  - `Machine.java`生成`Machine.class`虚拟机与`Machinetrace.class`堆栈追踪

- 测试集：测试程序放在`example`文件夹内

## 用法

- 以下操作均在microcc文件夹路径下进行

- `dotnet fslex --unicode CubyLex.fsl`  
  生成`CLex.fs`词法分析器

- `dotnet fsyacc --module CubyPar CubyPar.fsy`  
  生成`CPar.fs`语法分析器与`CubyPar.fsi`  

- `javac src/Machine.java`  
  生成虚拟机

  ### A 解释器

  #### A.1  解释器 interpc.exe 构建

-  **编译解释器 interpc.exe 命令行程序** 

  ```F#
  dotnet restore  interpc.fsproj   # 可选
  dotnet clean  interpc.fsproj     # 可选
  dotnet build -v n interpc.fsproj # 构建./bin/Debug/net5.0/interpc.exe ，-v n查看详细生成过程
  ```

- **执行解释器**

  ```F#
  ./bin/Debug/net5.0/interpc.exe example/ex1.c 8
  dotnet run -p interpc.fsproj example/ex1.c 8
  dotnet run -p interpc.fsproj -g example/ex1.c 8  //显示token AST 等调试信息
  ```

- **dotnet fsi 中运行解释器**

  ```F#
  # 生成扫描器
  dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fslex/netcoreapp3.1\fslex.dll"  -o "CLex.fs" --module CLex --unicode CLex.fsl
  
  # 生成分析器
  dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fsyacc/netcoreapp3.1\fsyacc.dll"  -o "CPar.fs" --module CPar CPar.fsy
  
  # 命令行运行程序
  dotnet fsi 
  
  #r "nuget: FsLexYacc";;  //添加包引用
  #load "Absyn.fs" "Debug.fs" "CPar.fs" "CLex.fs" "Parse.fs" "Interp.fs" "ParseAndRun.fs" ;; 
  
  open ParseAndRun;;    //导入模块 ParseAndRun
  fromFile "example\ex1.c";;    //显示 ex1.c的语法树
  run (fromFile "example\ex1.c") [17];; //解释执行 ex1.c
  run (fromFile "example\ex11.c") [8];; //解释执行 ex11.c
  
  Debug.debug <-  true  //打开调试
  
  run (fromFile "example\ex1.c") [8];; //解释执行 ex1.c
  run (fromFile "example\ex11.c") [8];; //解释执行 ex11.
  #q;;
  ```
  
- 解释器的主入口 是 interp.fs 中的 run 函数，具体看代码的注释

### B 编译器

#### B.1 microc编译器构建步骤

```sh
# 构建 microc.exe 编译器程序 
dotnet restore  microc.fsproj # 可选
dotnet clean  microc.fsproj   # 可选
dotnet build  microc.fsproj   # 构建 ./bin/Debug/net5.0/microc.exe

dotnet run -p microc.fsproj example/ex1.c    # 执行编译器，编译 ex1.c，并输出  ex1.out 文件
dotnet run -p microc.fsproj -g example/ex1.c   # -g 查看调试信息

./bin/Debug/net5.0/microc.exe -g example/ex1.c  # 直接执行构建的.exe文件，同上效果

dotnet built -t:ccrun microc.fsproj     # 编译并运行 example 目录下多个文件
dotnet built -t:cclean microc.fsproj    # 清除生成的文件
```

#### **B.2 dotnet fsi 中运行编译器**

```sh
# 启动fsi
dotnet fsi

#r "nuget: FsLexYacc";;

#load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "Machine.fs" "Backend.fs" "Comp.fs" "ParseAndComp.fs";;   

# 运行编译器
open ParseAndComp;;
compileToFile (fromFile "example\ex1.c") "ex1";; 

Debug.debug <-  true   # 打开调试
compileToFile (fromFile "example\ex4.c") "ex4";; # 观察变量在环境上的分配
#q;;


# fsi 中运行
#time "on";;  // 打开时间跟踪

# 参考A. 中的命令 比较下解释执行解释执行 与 编译执行 ex11.c 的速度
```

### C 优化编译器

#### C  优化编译器 microcc.exe 构建步骤

```sh
dotnet restore  microcc.fsproj
dotnet clean  microcc.fsproj
dotnet build  microcc.fsproj           # 构建编译器

dotnet run -p microcc.fsproj ex11.c    # 执行编译器
./bin/Debug/net5.0/microcc.exe ex11.c  # 直接执行
```

<!-- #### C.2 dotnet fsi 中运行 backwards编译器  

```sh
dotnet fsi -r ./bin/Debug/net5.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Contcomp.fs ParseAndComp.fs   

open ParseAndComp;;
contCompileToFile (fromFile "example\ex11.c") "ex11.out";;
#q;;
``` -->

### D 虚拟机构建与运行

虚拟机功能：

- `java Machine` 运行中间表示
- `java Machinetrace` 追踪堆栈变化

例子：

```sh
javac Machine.java # 构建虚拟机
java Machine example/ex1.out 8

javac Machinetrace.java # 构建虚拟机
java Machinetrace example/ex1.out 8
java Machinetrace example/ex1.out 8
```

## 功能实现

- 声明类型（编译器、解释器）

  - 简介：microcc里自带定义类型，我们为其添加了声明类型的功能

  - 例子

    ```
    int main(){
        int a=1;
        print a;
    }
    ```

    堆栈图

    ![declare](images/declare.png)
    
    解释器
    
    ![image-declare](images/image-declare.png)

------

- 数组类型（编译器、编译器）

  - 简介：int类型数组

  - 例子：

    ```
    int main(){
    
        int a = 10;
        int b[10];
    
        int i;
        a[0] = 1;
    
    }
    ```

    堆栈图：

    ![array](images/array.png)
    
    ```c
    int main(){
    
        int a = 10;
        int b[10];
    
        int i;
        b[0] = 1;
        print b[0];
    }
    ```
    
    解释器
    
    ![image-array](images/image-array.png)

------



- int类型（解释器、编译器）
  - 简介：整数类型
  
  - 例子 
    ```C
    // ex_int.c
    int g ;
    int h[3] ;
    void main(int n) {
  int a;
      a=1;
      print a;
    }
    

    - 堆栈图  
    
    ![int](images/int.png)
  
   - 解释器
  
  ![image-int](images/image-int.png)

- isInt，ischar函数（编译器）

  - 简介：判断变量或数字是否为int类型，ischar用于判断是否为字符类型

  - 例子：

    ```
    int main(){
        int a = IsInt(3);
        int b = IsInt(3.3);
        int f = IsChar('e');
        print a;
        print b;
        print f;
    }
    ```

    堆栈图：

    ![isInt](images/isInt.png)

------



- float 类型（编译器、解释器）
  
  - 简介：浮点型类型，我们将原本的小数转化为二进制表示，再将这二进制转化为整数放入堆栈中，在虚拟机中再转化为小数。
  - 例子：
    - 例一：   
        ```C
        int main(){
            float a;
            a = 1.1; 
            print a;
        }
        ```
        
    - 运行栈追踪：  
    ![float1](images/float1.png)
    
    - 解释器
    
    ![image-float](images/image-float.png)
    
    - 例二：
        ```C
        int main(){
            float a;
            int b;
            a = 1.1; 
            b = 1;
            print a+b;
            print a;
        }
        ```
        
    - 运行栈追踪：  
        ![float2](images/float2.png)

---

- char 类型（编译器，解释器）
  
  - 简介：字符类型
  
- 例子：
  
    ```C
    void main() {
       char a;
       a = 'a';
     print a;
  }
  ```
  
  - 运行栈追踪：  
    ![char](images/char.png)
    
  - 解释器
  
  ![image-char](images/image-char.png)

---
- ToInt,ToChar,ToFloat类型转换（编译器）

  - 简介：实现了三种类型转换的操作，ToInt,ToChar,ToFloat函数

  - 例子：

    ```
    int main(){
        char a;
        a='a';
        int b;
        b=ToInt(3.14);
        int c;
        c=ToInt('9');
        // int d = 97;
        char e ;
        e = ToChar(97);
        float f;
        f = ToFloat(97);
        print b;
        print c;
        print e;
        print f;
    }
    ```

    堆栈图：

    ![toInt](images/toInt.png)

------



- 自增操作（解释器、编译器）
  
    - 简介:i++与++i操作
    
    - 例子：
        ```C
        void main() {
          int a;
          int n;
          a=1;
          n=a++;
          print a;
        }
        
        ```
        
    - 运行栈追踪：  
        ![selfplus](images/selfplus.png)
        
    - 解释器
    
    ![image-selfplus](images/image-selfplus.png)
---
- 自减操作（解释器、编译器）

  - 简介：i--与--i操作

  - 例子

    ```c
    void main() {
      int a;
      int n;
      a=3;
      a--;
      --a;
      print a;
      return 0;
    }
    ```

    运行栈追踪：

    ![selfminus](images/selfminus.png)
    
    解释器
    
    ![image-selfminus](images/image-selfminus.png)

------



- FOR循环（解释器、编译器）
  
    - 简介：for()函数以及for n in ()函数
    
    - 例子1：
        ```C
        int main(){
            int i;
            i = 0;
            int n;
            n = 0;
            for(i =0 ; i < 5 ;  i=i+1){
                n = n + i;
            }
            print n;
        }
        ```
        
    - 运行栈追踪：  
      
    - ![for1](images/for1.png)
      
    - ![for2](images/for2.png)
    
    - ![for3](images/for3.png)
    
    - 解释器
    
    ![image-for](images/image-for.png)
    
    - 例子2：
    
        ```
        int main()
        {
            int n;
            int s;
            s = 0;
            for n in (3..7)
            {
                s = s+n;
                print s;
            }
        }
        ```
    
        堆栈图：
    
        ![range1](images/range1.png)
    
        ![range2](images/range2.png)
    
        ![range3](images/range3.png)
        
        解释器
        
        ![image-range](images/image-range.png)
---
- dountil函数（解释器、编译器）

  - 简介：与dowhile相反的功能，循环，直到until里的内容符合条件

  - 例子：

    ```
    int main(){
        int n;
        n = 0;
        int s;
        s = 0;
        do{
            s = s + n;
            n = n + 1;
        }until(n>=5);
        print s;
        print n;
    }
    ```

    堆栈图：

    ![until](images/until.png)

    ![until2](images/until2.png)

    ![until3](images/until3.png)
    
    解释器
    
    ![image-until](images/image-until.png)

------

- while循环（解释器、编译器）

  - 简介：普通的while循环，先判定while内的条件，后运行body的内容，直到while不满足

  - 例子:

    ```
    int main(){
        int n;
        n = 0;
        int s;
        s = 0;
        while(n<5){
            s = s + n;
            n = n + 1;
        }
        print s;
        print n;
    }
    ```

    堆栈图：

    ![while](images/while.png)

    ![while2](images/while2.png)

    ![while3](images/while3.png)

    ![while4](images/while4.png)
    
    解释器
    
    ![image-while](images/image-while.png)

------

- if，elseif条件判断符（编译器、if解释器）

  - 简介：判断if内的条件，若满足则运行body内的内容，不满足继续判断else if的内容，直到最后。

  - 例子：

    ```
    int main(){
        int a = 2;
        if (a == 1){
            print 1;
        }else if(a == 3){
            print 3;
        }else if(a == 4){
            print 4;
        }else if(a == 2){
            print 2;
        }
    }
    ```

    堆栈图：

    ![if](images/if.png)

------



- 三目运算符（解释器、编译器）
  
    - 简介：三目运算符 a>b?a:b
    
    - 用例：
        ```C
        int main()
        {
            int a=0;
            int b=7;
            int c = a>b?a:b;
            print c;
        }
        ```
        
    - 运行栈追踪：  
      
        ![prim3](images/prim3.png)
        
        解释器
        
        ![image-prim3](images/image-prim3.png)
---
- do - while（解释器、编译器）
    - 简介：在判断前先运行body中的操作。
    - 例子：
        ```C
        int main(){
            int n;
            n = 0;
            int s;
            s = 0;
            do{
                s = s + n;
                n = n + 1;
    }while(n<5);
          print s;
          print n;
      }
      ```
      
  - 运行栈追踪：
    - 堆栈图：
    ![dowhile1](images/dowhile1.png)
    - ![dowhile2](images/dowhile2.png)
    - ![dowhile3](images/dowhile3.png)
    - 解释器
    
    ![image-dowhile](images/image-dowhile.png)
---
- 类似C的switch-case（解释器、编译器）
    - 当没有break时，匹配到一个case后，会往下执行所以case的body
    - 若当前没有匹配的case时，不会执行body，会一直往下找匹配的case
    - 之前的实现是递归匹配每个case，当前类似C语言的switch-case实现上在label的设立更为复杂一些。
    - 例子：
        ```C
        int main(){
            int i=0;
            int n=1;
            switch(n){
                case 1:{i=n+n;i=i+5;}
                case 5:i=i+n*n;
            }
        }
        ```

  - 运行栈追踪：
    - n的值与case1 匹配，没有break， i=n+n与case 5 中的i+n*n都被执行
    - i的结果为（1+1）+1*1 = 3
    - 栈中3的位置为i，4的位置为n
    - 堆栈图：  
    ![switch1](images/switch1.png)
    - 解释器
    
    ![image-switch](images/image-switch.png)

---

- break功能（编译器）
    - 在for while switch 中，都加入break功能
    - 维护Label表来实现
    - 例子：与没有break的switch进行对比：
        ```C
        int main(){
            int i=0;
            int n=1;
            switch(n){
                case 1:{i=n+n;break;}
                case 5:i=i+n*n;
            }
            print i;
        }
      ```
  - 运行栈追踪
    - n的值与case1 匹配，执行i=n+n，遇到break结束。
    - i的结果为（1+1）=2
    - 栈中3的位置为i，4的位置为n
    - 堆栈图：  
        ![break](images/break.png)

---
- continue 功能（编译器）
    - 在for while 中加入continue功能
    - 例子：
        ```C
        int main()
        {
            int i ;
            int n = 0;
            for(i=0;i<5;i++)
            {
                if(i<2)
                    continue;
                if(i>3)
                    break;
                n=n+i;
            }
            print n;
        }
      ```
  - 运行栈追踪：
    - i=0 1 的时候continue i>3 的时候break
    - n = 2 + 3 结果为5
    - 栈中3的位置为i， 4的位置为n
    - 堆栈图：  
    ![continue](images/continue.png)
    - ![continue2](images/continue2.png)
    - ![continue3](images/continue3.png)

---

- JVM
  - 简介：
    - 将之前的虚拟机重新写了一边，原先的虚拟机只能针对int类型进行存储和操作。我具体定义了一些类型，使虚拟机具有较强的拓展性。
  - 类型继承关系图：
    - ![](img/JVM.png)
  - 指令集添加：
    - CSTF：
      - 简介：const float
      - 功能：在堆栈中加入一个float
    - CSTC： 
      - 简介：const char
      - 功能：在堆栈中加入一个char
  - 运行时异常：
    - OperatorError:
      - 简介：在计算时发生的异常，例如除数为0时，出现异常。
    - ImcompatibleTypeError:
      - 简介：类型不匹配的时候进行时出现的异常，例如'a' + 1.0 抛出异常。

---

- try-catch：（编译器）
  - 简介：
    - 除0异常捕获：
      - 虚拟机运行时的除0异常
      - 出现显式的除0时THROW 异常
    - 寄存器添加：
      - hr： 保存当前异常在栈中的地址，用于追踪需要处理的异常
    - 指令添加：
      - PUSHHDLR,保存catch中的异常种类，需要跳转的位置以及hr入栈
      - POPHDLR ，与PUSHHDLR对应
      - THROW   ，用于丢出异常，从hr开始找匹配
    - 例子：
  - 目前做了有关除0异常的内容，由于没有异常类，暂且通过字符串的方式作为异常种类，将异常编号匹配。解决了运行时try-catch变量环境的问题，解决了异常处理时栈环境变化带来的空间影响。能够正常的匹配到异常。
    ```C
        int main()
        {
            try{
                int a=0;
                int n=5;
                n=n/a;
            }
            catch("ArithmeticalExcption")
            {
                n=0;
                print n;
            }   
        }
    ```
    - 运行时堆栈：  

- ![catch](images/catch.png)    

------

- 结构体功能（编译器）

  - 简介：加入了C中的结构体功能
  - 首先，先创建结构体定义表，用来查找，结构体定义表中包含结构体的总体大小，名字，以及变量和偏移量。然后查找结构体变量表，加入该变量到varEnv中，访问成员时，便可以通过`.`运算符，通过偏移值转化为简单指令集。
  - 例子：

  ```c
  struct student{
      int age;
      float id;
  };
  int main(){
      struct student hello;
      hello.age = 10;
      hello.id = 234.1;
      print hello.age;
      print hello.id;
  }
  ```

  - ​	运行栈追踪

  ![image-int](images/image-int.png)

------

- function（编译器）

  - 简介：实现了函数的定义和返回
  - 例子：

  ```c
  int fact(int i){
      if(i == 1){
          return 1;
      }else{
          return i * fact(i - 1);
      }
  }
  
  int main(){
      int n = 10;
      int b;
      b=fact(n);
      print b;
  }
  ```

  - 运行栈追踪

 
![image-ex1-](images/image-ex1-.png)
  ![image-20220627223313111](images/image-20220627223313111.png)

  ![image-20220627223327952](images/image-20220627223327952.png)

  ![image-20220627223341264](images/image-20220627223341264.png)

  ![image-20220627223354185](images/image-20220627223354185.png)

------

- return 0解释器

  - 简介：实现了return 0的解释器功能，未能完成funEnv的添加，所以函数返回值无法传递。

  ```c
  void main(int n) {
    while (n > 0) {
      print n;
      n = n - 1;
    }
  
    println;
    return 0;
  }
  ```

  ![image-ex1](images/image-ex1.png)


## 心得体会
- 孙国豪：  
  
  ​		这门课给我最大的感受就是前两节课感觉还挺容易听的明白，但是越到后面感觉越迷糊。感觉越来越难听懂，然后通过老师的讲解和同学的帮助，大致能掌握一些知识。在这门课上，我不仅了解了编译原理的一些知识，也顺带学会了Fsharp，不得不说，Fsharp的代码风格真的很适合写编译器，给我们在后续大作业的完成上省去了很多麻烦。
  
  ​		完成大作业的过程，其实就是逐渐理解的过程，因为开始课上对这些概念其实有些模棱两可。在刚拿到大作业的时候感觉还是很难下手，然后通过参考老师提供的Cuby的代码，我逐渐开始知道优化Microc应该从哪几个文件开始下手，每个文件的功能是什么，联系是什么。可以说这份参考文档给了我们很大的帮助。我也从开始改char需要花一个下午的时间到后面一个功能只需花几十分钟，感觉真的是非常有成就感的一个过程。
  
  ​		现在我不能说对编译原理理解颇深，起码来说可以略懂一二吧，学这门课还是非常有收获的，老师上课也给了我们很多的参考资料，上课也讲的非常清楚。
  
- 舒以恒：  

  这学期学了编译原理这门课，第一次接触到F#这个语言，对我来说，F#相比其他编程语言是一个完全没接触过的全新类型的语言，而且网上资料也很少，一切都得自己慢慢摸索，学习起来也很吃力，需要投入相当多的精力去了解，去学习编译之中的理论以及实践的内容。在课程期间学习的编译原理的基本理念以及大作业对microcc的改进与增添功能，感觉都对计算机学习有着极大的帮助。
  
  总结一下：大作业完成过程中，了解了函数式编程的语言特点与特性，清楚了一些关于C语言的设计方法与局限性。对指令与栈结构有了更深的了解，理解了栈式虚拟机的工作原理与一些设计方法。并且在使用F#的过程中，逐渐了解一些编译的理念，也加深了对函数式编程语言的印象。总的来说对我理解编译语言带来了很大的帮助，也让我成长了很多。


## 技术评价

| 功能 | 对应文件 | 优  | 良  | 中  |
| ---- | -------- | --- | --- | --- |
|变量声明|ex_declare.c|√|||
|数组类型|ex_array.c|√|||
|float类型|ex_float.c|√|||
|自增|ex_selfplus.c|√|||
|自减|ex_selfminus.c|√|||
|for循环|ex_for.c|√|||
|int类型|ex_int.c|√|||
|char类型|ex_char.c|√|||
|isInt,isChar函数|ex_isint.c||√||
|ToInt,ToChar,ToFloat|ex_toint.c|√|||
|三目运算符|ex_prim3.c|√|||
|do-while|ex_dowhile.c|√|||
|do-until|ex_until.c|√|||
|while|ex_while.c|√|||
|range循环|ex_range.c|√|||
|break|ex_break.c|√|||
|continue|ex_continue.c|√|||
|switch-case|ex_switch.c|√|||
|if,else if|ex_if.c|√|||
|struct|ex_struct.c||√||
|try-catch|ex_try.c||√||
|虚拟机类型支持|Machine.java|√|
|虚拟机异常|exception|√

## 小组分工

- 孙国豪（组长）
  - 学号：31901053
  - 班级：计算机1902
    - 工作内容
      - 测试程序
      - 语法分析
      - 词法分析
      - 栈、堆设计
- 舒以恒
  - 学号：31901051
  - 班级：计算机1902
    - 工作内容
      - 文档编写
      - 测试程序
      - 主要负责虚拟机和中间代码生成
      - 部分功能实现
  
- 权重分配表：  

| 孙国豪 | 舒以恒 |
| ------ | ---- |
| 0.95   | 0.95 |

