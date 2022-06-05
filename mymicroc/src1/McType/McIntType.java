package McType;

public class McIntType extends McBaseType {
    private int value;

    public McIntType(){
        value = 0;
    }

    public McIntType(int value){
        this.value = value;
    }

    public int getValue() {
        return value;
    }

    public void setValue(int value) {
        this.value = value;
    }


}
