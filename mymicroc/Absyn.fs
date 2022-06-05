(* File MicroC/Absyn.fs
   Abstract syntax of micro-C, an imperative language.
   sestoft@itu.dk 2009-09-25

   Must precede Interp.fs, Comp.fs and Contcomp.fs in Solution Explorer
 *)

module Absyn

// 基本类型
// 注意，数组、指针是递归类型
// 这里没有函数类型，注意与上次课的 MicroML 对比
type typ =
  | TypI                             (* Type int                    *)
  | TypC                             (* Type char                   *)
  | TypS                             (* Type string                 *)
  | TypF                             (* Type float                  *)
  | TypeStruct of string
  | TypA of typ * int option         (* Array type                  *)
  | TypP of typ                      (* Pointer type                *)
                                                                   
and expr =                           // 表达式，右值                                                
  | Access of access                 (* x    or  *p    or  a[e]     *) //访问左值（右值）
  | Assign of access * expr          (* x=e  or  *p=e  or  a[e]=e   *)
  | Addr of access                   (* &x   or  &*p   or  &a[e]    *)
  | CstI of int                      (* Constant                    *)
  | CstS of string                   (* Constant String             *)
  | CstF of float32                  (* Constant float              *)
  | CstC of char                     (* Constant Char               *)
  | Prim1 of string * expr           (* Unary primitive operator    *)
  | Prim2 of string * expr * expr    (* Binary primitive operator   *)
  | Prim3 of expr * expr * expr
  | Andalso of expr * expr           (* Sequential and              *)
  | Orelse of expr * expr            (* Sequential or               *)
  | Call of string * expr list       (* Function call f(...)        *)
  | Var of var
  | ToInt of expr
  | ToChar of expr
  | ToFloat of expr
  | IsInt of expr
  | IsFloat of expr
  | IsChar of expr


and access =                         //左值，存储的位置                                            
  | AccVar of string                 (* Variable access        x    *) 
  | AccDeref of expr                 (* Pointer dereferencing  *p   *)
  | AccIndex of access * expr        (* Array indexing         a[e] *)
  | AccessMember of access * access  (**)

and stmt =                                                         
  | If of expr * stmt * stmt         (* Conditional                 *)
  | While of expr * stmt             (* While loop                  *)
  | DoWhile of stmt * expr
  | DoUntil of stmt * expr
  | Expr of expr                     (* Expression statement   e;   *)
  | Return of expr option            (* Return from method          *)
  | Block of stmtordec list          (* Block: grouping and scope   *)
  | For of expr * expr * expr * stmt (* normal for *)
  | Range of expr * expr * expr * stmt
  | Case of expr * stmt
  | Switch of expr * stmt list
  | Throw of IException
  | Try of stmt * stmt list
  | Catch of IException * stmt
  | READ of var
  | Break
  | Continue
  // 语句块内部，可以是变量声明 或语句的列表                                                              

and var=string

and IException = 
  | Exception of string
    
and stmtordec =                                                    
  | Dec of typ * string              (* Local variable declaration  *)
  | Stmt of stmt                     (* A statement                 *)
  | DeclareAndAssign of typ * string * expr (*变量赋值*)

// 顶级声明 可以是函数声明或变量声明
and topdec = 
  | Fundec of typ option * string * (typ * string) list * stmt
  | Vardec of typ * string
  | VariableDeclareAndAssign of typ * string * expr (*变量赋值*)
  | StructDeclare of  string * (typ * string) list  (*结构声明*)

// 程序是顶级声明的列表
and program = 
  | Prog of topdec list
