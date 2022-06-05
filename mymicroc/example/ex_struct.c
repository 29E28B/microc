struct student{
    int age;
    float id;
    struct info{
        int phone;
    };
};
int main(){
    struct student hello;
    hello.age = 10;
    hello.id = 234.1;
    hello.info.phone = 12;
    print hello.age;
    print hello.id;
    print hello.info.phone;
}