function Sleep(sec : number) {
    return new Promise(resolve => setTimeout(resolve, sec));
}

async function test()
{
    console.log("step 1")
    await Sleep(2000)
    console.log("step 2")
}

test()
console.log("back in main")