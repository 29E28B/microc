package McType;
public class McCharType extends McBaseType{
    private char value;

    McCharType(){
        value = 0;
    }

    public McCharType(char c){
        value = c;
    }

    public char getValue() {
        return value;
    }

    public void setValue(char value) {
        this.value = value;
    }
}
