function handleClickAddStep(){
    const index = todoEditVM.steps().findIndex(s=>s.isNew);
    // if(index !== -1){
    //     return;
    // }
    todoEditVM.steps.push(new stepViewModel({isEditing:true, isCompleted:false}));
    $("[name=txtStepDescription]:visible").focus();
}

function handleClickCancelStep(step){
    if(step.isNew()){
        todoEditVM.steps.pop();
    } else {
        step.isEditing(false);
        step.description(step.lastDescription);
    }
}

async function handleClickSaveStep(step){
    step.isEditing(false);
    const isNew = step.isNew();
    console.log(todoEditVM);
    const todoId = todoEditVM.id;
    const data = getBodyStep(step);
    
    const description = step.description();
    
    if(!description){
        step.description(step.lastDescription);
        if(isNew){
            todoEditVM.steps.pop();
        }
        return;
    }
    
    if(isNew){
        await insertStep(step,data,todoId);
    } else {
        await updateStep(data,step.id());
    }
}

async function insertStep(step,data,todoId){
    const response = await fetch(`${urlSteps}/${todoId}`,{
        body:data,
        method:"POST",
        headers:{
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const json = await response.json();
        step.id(json.id);
        
        const todo = getTodoEditing();
        todo.stepsTotal(todo.stepsTotal()+1);
        
        if(step.isCompleted()){
            todo.stepsDone(todo.stepsDone()+1);
        }
    } else {
        handleApiError(response);
    }
}


function getBodyStep(step){
    return JSON.stringify({
        description: step.description(),
        isCompleted: step.isCompleted()
    });
}


function handleClickDescriptionStep(step){
    step.isEditing(true);
    step.lastDescription =step.description();
    $("[name=txtStepDescription]:visible").focus();
}

async function updateStep(data,id){
    const response = await fetch(`${urlSteps}/${id}`,{
        body:data,
        method:"PUT",
        headers:{
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok)
    {
        handleApiError(response);
    }
}

function handleClickCheckboxStep(step){
    if(step.isNew())
        return true;
    
    const data = getBodyStep(step);
    updateStep(data,step.id());
    
    const todo = getTodoEditing();
    let currentStepDone = todo.stepsDone();
    
    if(step.isCompleted()){
        currentStepDone++;
    }else{
        currentStepDone--;
    }
    
    todo.stepsDone(currentStepDone);
    
    return true;
}

function handleClickDeleteStep(step) {
    modalTodoEditBootstrap.hide();
    confirmAction({
        callBackAcept: () => {
            deleteStep(step);
            modalTodoEditBootstrap.show();
        },
        callBackCancel: ()=>{
            modalTodoEditBootstrap.show();
        },
        title: `Desea borrar este paso?`
    })
}

async function deleteStep(step){
    const response = await fetch(`${urlSteps}/${step.id()}`,{
        method: 'DELETE'
    });
    
    if(!response.ok){
        handleApiError(response);
        return;
    }
    
    todoEditVM.steps.remove(function(item) {return item.id == step.id});
    
    const todo = getTodoEditing();
    todo.stepsTotal(todo.stepsTotal()-1);
    
    if(step.isCompleted())
        todo.stepsDone(todo.stepsDone()-1);
    
}

async function updateStepOrder(){
    const ids= getStepsIds();
    await sendStepsIdsToBackend(ids);
    
    const arraySorted  = todoEditVM.steps.sorted(function(a,b){
        return ids.indexOf(a.id().toString())-ids.indexOf(b.id().toString());
    });
    
    todoEditVM.steps(arraySorted);
}
function getStepsIds(){
    const ids = $("[name=chbStep]").map(function(){
        return $(this).attr("data-id");
    }).get();
    
    return ids;
}

async function sendStepsIdsToBackend(ids){
    var data = JSON.stringify(ids)
    await fetch(`${urlSteps}/order/${todoEditVM.id}`,{
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function(){
    $("#draggable-steps").sortable({
        axis: 'y',
        stop: async function(){
            await updateStepOrder();
        }
    })
})

