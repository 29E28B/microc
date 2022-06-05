package McType;

public class McArrayType extends McBaseType{
    private McBaseType[] value;
    private int length;

    public McArrayType(){
        value = null;
    }

    public McArrayType(McBaseType[] array){
        value = array;
    }

    public McBaseType[] getValue() {
        return value;
    }

    public void setValue(McBaseType[] value) {
        this.value = value;
    }
}
