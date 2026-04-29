class UlmItem {
    constructor() {
        this.loc = "Ulm";
        this.print = function () { console.log(`${this.name} at ${this.loc}`); };
    }
}

class Person extends UlmItem {
    constructor(name, age) {
        super();
        this.name = name;
        this.age = age;
    }
}

var p = new Person("Hans", 12);
p.print();