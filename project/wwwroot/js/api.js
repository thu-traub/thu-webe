let APIURL = "/api/person";

var API = {
    getPersons: async function () {
        let response = await fetch(`${APIURL}`);
        return await response.json();
    },
    getPerson: async function (id) {
        let response = await fetch(`${APIURL}/${id}`);
        return await response.json();
    },
    updatePerson: async function (person) {
        let response = await fetch(`${APIURL}/${person.id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(person)
        });
        return await response.json();
    }   
}
