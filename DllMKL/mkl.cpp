#include "pch.h"
#include "mkl.h"




// Enum to represent the status of the interpolation
enum InterpolationStatus {
    SUCCESS = 0,
    TASK_CREATION_ERROR,
    SPLINE_SETUP_ERROR,
    SPLINE_CONSTRUCTION_ERROR,
    INTERPOLATION_ERROR,
    INTEGRATION_ERROR,
    TASK_DELETION_ERROR,
    UNKNOWN_ERROR
};

extern "C" _declspec(dllexport) 
void CubeInterpolate(
    MKL_INT num_points_x, MKL_INT num_points_y, double* x_values, double* y_values, double* boundary_conditions,
    double* spline_coefficients, MKL_INT num_sites, double* sites, MKL_INT num_derivatives,
    MKL_INT * derivative_orders, double* interpolated_values, InterpolationStatus & return_status,
    MKL_INT nlim, double* left_limit, double* right_limit, double* integral_result, bool isUniform)

{
    try
    {
        int status;
        DFTaskPtr task;

        status = dfdNewTask1D(&task, num_points_x, x_values, isUniform ? DF_UNIFORM_PARTITION : DF_NON_UNIFORM_PARTITION, 
            num_points_y, y_values, DF_MATRIX_STORAGE_ROWS);
        if (status != DF_STATUS_OK)
        {
            return_status = TASK_CREATION_ERROR;
            return;
        }

        status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_1ST_LEFT_DER | DF_BC_1ST_RIGHT_DER,
            boundary_conditions, DF_NO_IC, NULL, spline_coefficients, DF_NO_HINT);
        if (status != DF_STATUS_OK)
        {
            return_status = SPLINE_SETUP_ERROR;
            return;
        }

        status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
        if (status != DF_STATUS_OK)
        {
            return_status = SPLINE_CONSTRUCTION_ERROR;
            return;
        }

        status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, num_sites, sites, isUniform ? DF_UNIFORM_PARTITION : DF_NON_UNIFORM_PARTITION, 
            num_derivatives, derivative_orders, NULL, interpolated_values, DF_MATRIX_STORAGE_ROWS, NULL);
        if (status != DF_STATUS_OK)
        {
            return_status = INTERPOLATION_ERROR;
            return;
        }

        status = dfdIntegrate1D(task, DF_METHOD_PP, nlim, left_limit, DF_NO_HINT, right_limit, DF_SORTED_DATA, NULL, NULL,
            integral_result, DF_MATRIX_STORAGE_ROWS);
        if (status != DF_STATUS_OK)
        {
            return_status = INTEGRATION_ERROR;
            return;
        }

        status = dfDeleteTask(&task);
        if (status != DF_STATUS_OK)
        {
            return_status = TASK_DELETION_ERROR;
            return;
        }

        return_status = SUCCESS;
    }
    catch (...)
    {
        return_status = UNKNOWN_ERROR;
    }
}