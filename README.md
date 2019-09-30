# Fly Language
Fly is dynamically typed interpreted programming language. You can use the following types in Fly:
`int`, `float`, `string`, `array` and `bool`. All variables can be any type but not all types are automatically converted to one another.
Here you can see a few variable assignment and type examples:
```
my_int = 100;
my_float = 1.53;
my_string = "Hello World!";
my_array = [1, 2, 3, 5, 7, 11, 13, 17];
no_value = nil;
my_bool = false;
```
All variable values except `false` and `nil` are also true when put in a condition statement (if, elif, else, for).
There are 4 kinds of `block` statements (`if`, `elif`, `else` and the `for` loop):
```
if my_int { 
    // if my int is not false/nil
    print(my_int);
}
elif my_int == 99 {
    print("This is a elif statement");
}
else{
    print("my_int is something else");
}
i = 0;
for i < 10 {
    print(i);
    i += 1;
}
```
## Arrays
You can use (dynamic) arrays to store multiple values in 1 type. Arrays have a dynamic size which changes as items are added. You can also define variables with a different initial size than the default (24), this is good to use for better performance on a lot of array operations. Here are some array examples:
```
fruits = ["Apple", "Pear", "Lime", "Banana"];
print("I know " + fruits.count() + " fruits!");
numbers = [](100, 10); // This array has a length of 100 and adds 10 items when it resizes.
i = 0;
for i < 100 {
    numbers[i] = "item " + i;
    i += 1;
}
```
## Functions
Here are some functions examples:
```
box add_2_items(a, b){
    return a + b;
}
print(add_2_items(10, 30)); // 40
```
## Builtins
Fly has a few builtin libraries and functions:
`print(arg1)` prints arg1 to the console
`print("Hello World") // Hello World`
`input()` reads a line from the console input and returns it as a string
`line = input() // reads a line`
`obj.count()` counts the items in a variable (with obj as variable)
`[1, 2, 3].count() // 3`
### Libraries
import the libraries using and import statement; for example:
`import math, text`
#### `math`
usage example:
```number = math.power(5, 2); // 25
floor(float)
ceiling(float)
sin(float)
cos(float)
tan(float)
asin(float)
acos(float)
atan(float)
sqrt(float)
log(float)
logx(float y, float x)
log10(float)
power(float base, float exponent)
power(float decimal, int decimalDigits)
```
#### `text`
usage example:
```
items = text.split("1,2,3,4", ","); // ['1', '2', '3', '4']
split(string input, string seperator);
replace(string source, string target, string replaceWith)
join(array items, string separator);
```
